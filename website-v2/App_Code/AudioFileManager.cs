using System;
using System.IO;
using OttaMatta.Application;
using OttaMatta.Common;
using OttaMatta.Data;
using WaveLib;
using Yeti.MMedia.Mp3;

/// <summary>
/// Class to manage the local sound file cache
/// </summary>
public class AudioFileManager
{
    private PlayerRequestInfo _requestInfo = null;
    private string _encryptedPlayerRequest = null;
    private string _targetPath = null;
    private dynamic _soundData = null;

    public string ErrorMsg { get; set; }
    public string LocalWavFile { get; set; }
    public string LocalMp3File { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="encryptedPlayerRequest">The encrypted sound parameter</param>
    /// <param name="targetPath">The path to the local sound cache directory</param>
	public AudioFileManager(string encryptedPlayerRequest, string targetPath)
	{
        _targetPath = targetPath;
        _encryptedPlayerRequest = encryptedPlayerRequest;
        _requestInfo = new PlayerRequestInfo(encryptedPlayerRequest);

        //
        // After all is said and done, these files should be created
        //
        LocalMp3File = Functions.CombineElementsWithDelimiter("\\", _targetPath, string.Format("id{0}.mp3", _requestInfo.SoundId));
        LocalWavFile = Functions.CombineElementsWithDelimiter("\\", _targetPath, string.Format("id{0}.wav", _requestInfo.SoundId));
	}

    /// <summary>
    /// Get the sound from the database if we don't already have it
    /// </summary>
    /// <param name="soundid">The sound id</param>
    /// <returns>The sound data</returns>
    private dynamic GetSoundData(int soundid)
    {
        if (_soundData == null)
        {
            _soundData = DataManager.GetSoundData(soundid);
        }

        return _soundData;
    }

    /// <summary>
    /// See if we have an existing file of the passed type
    /// </summary>
    /// <param name="type">The type to check for</param>
    /// <returns>True if we have it</returns>
    public bool HaveFileOfType(PlayerRequestInfo.AudioFileTypeCode type)
    {
        switch (type)
        {
            case PlayerRequestInfo.AudioFileTypeCode.Mp3:
                return File.Exists(LocalMp3File);
            case PlayerRequestInfo.AudioFileTypeCode.Wav:
                return File.Exists(LocalWavFile);
            default:
                throw new NotImplementedException(string.Format("Unknown audio type: {0}", type));
        }
    }

    /// <summary>
    /// Do we have to create any files?
    /// </summary>
    /// <param name="mp3File">The path to the mp3 file</param>
    /// <param name="wavFile">The path to the wav file</param>
    /// <returns></returns>
    public bool NeedToCreateAtLeastOneFile(string mp3File, string wavFile)
    {
        return !(File.Exists(mp3File) && File.Exists(wavFile));
    }

    public static bool HaveAtLeastOneFile(string mp3File, string wavFile)
    {
        return File.Exists(mp3File) || File.Exists(wavFile);
    }

    public bool HaveAtLeastOneFile()
    {
        return HaveAtLeastOneFile(LocalMp3File, LocalWavFile);
    }

    /// <summary>
    /// Build all the temp files
    /// </summary>
    /// <remarks>
    /// One of the most hideous functions ever written.  Refactoring would be nice.
    /// </remarks>
    public void CreateTempFiles()
    {
        /* Dev links:
         * 
         * http://localhost:82/player/epgcbha - clerks mp3 - id46
         * 
         * http://localhost:82/player/dpccbha - beavis wav - id32
         * 
         * http://localhost:82/player/dpbcbha - ace losers mp3 - id31
         * 
         * http://192.168.1.117:82/player/fqavbha - borat wav - id50 - it is nice - won't convert to mp3
         * 
         */


        dynamic theSound = null;
        Stream outputStream = new MemoryStream();

        if (_requestInfo.IsValid)   // Functions.IsNumeric(soundId))
        {
            int soundId = _requestInfo.SoundId;

            //
            // First, look in the cache and see if we have a converted mp3 there already
            //
            string targetMp3File = LocalMp3File;    //Functions.CombineElementsWithDelimiter("\\", _targetPath, string.Format("id{0}.mp3", soundId));
            string targetWavFile = LocalWavFile;    // Functions.CombineElementsWithDelimiter("\\", _targetPath, string.Format("id{0}.wav", soundId));

            //if (!NeedToCreateAtLeastOneFile(targetMp3File, targetWavFile))
            if (HaveAtLeastOneFile(targetMp3File, targetWavFile))
            {
                return;
            }
            else
            {
                //
                // OK, let's grab the sound data from the DB
                //
                theSound = GetSoundData(Functions.ConvertInt(soundId, -1));

                if (theSound != null && theSound.Id >= 0)
                {
                    //
                    // The DB returns HTML-ready bytes.  It would be more efficient to return the binary data.  Then we wouldn't have to do 2 conversions.
                    // Todo!
                    //
                    byte[] originalSourceByteArray = System.Convert.FromBase64String(theSound.Data);

                    if (Functions.IsWav(theSound.FileName))
                    {
                        bool successfulWavConversionToMp3 = false;

                        //
                        // It's an wav, convert to mp3
                        //
                        Mp3Writer outputMp3Writer = null;
                        // Mp3Writer outputFileMp3Writer = null;
                        
                        //
                        // These are WaveLib.WaveStream objects that wrap the LAME thing
                        //
                        WaveStream waveDataToConvertToMp3 = null;
                        WaveStream convertedSourceWaveStream = null;
                        WaveStream originalSourceWaveStream = new WaveStream(new MemoryStream(originalSourceByteArray));

                        try
                        {
                            outputMp3Writer = new Mp3Writer(outputStream, originalSourceWaveStream.Format);
                            waveDataToConvertToMp3 = originalSourceWaveStream;
                        }
                        catch         // (Exception ex)
                        {
                            outputMp3Writer = null;

                            //
                            // The source WAV isn't compatible with the LAME thingy.  Let's try to convert it to something we know is usable with the NAudio thingy.
                            // Then we'll use the NAudio stuff to try to get the LAME stuff to work.
                            //
                            //MemoryStream tempMemStream = new MemoryStream();
                            int sampleRate = 16000;
                            int bitDepth = 16;
                            
                            //
                            // Note: there appears to be a bug in the LAME thing for files with 1 channel (mono).  The file plays at double speed.  
                            //
                            int channels = 2;
                            NAudio.Wave.WaveFormat targetFormat = new NAudio.Wave.WaveFormat(sampleRate, bitDepth, channels);

                            NAudio.Wave.WaveStream stream = new NAudio.Wave.WaveFileReader(new MemoryStream(originalSourceByteArray));
                            NAudio.Wave.WaveFormatConversionStream str = null;

                            try
                            {
                                str = new NAudio.Wave.WaveFormatConversionStream(targetFormat, stream);
                            }
                            catch (Exception ex3)
                            {
                                //
                                // The borat "It is nice" WAV won't convert, has strange exception.  Todo: add logging and fix.
                                //
                                ErrorMsg = string.Format("Well, naudio can't convert the WAV to the target WAV format either: {0}", ex3.Message);
                            }

                            if (str != null)
                            {
                                //
                                // For lack of a better solution to get the bytes from the converted data into a "WaveStream" variable, use these
                                // available methods to write to a disk file and then open up the disk file.  The problem with directly converting
                                // with memory streams is that the required headers (like "RIFF") aren't written into the converted stream at this point.  
                                // 
                                NAudio.Wave.WaveFileWriter.CreateWaveFile(targetWavFile, str);
                                convertedSourceWaveStream = new WaveStream(targetWavFile);

                                //
                                // Now we have a correct WAV memory stream
                                //
                                try
                                {
                                    WaveFormat format = convertedSourceWaveStream.Format;
                                    outputMp3Writer = new Mp3Writer(outputStream, format);
                                    waveDataToConvertToMp3 = convertedSourceWaveStream;
                                }
                                catch (Exception ex2)
                                {
                                    //
                                    // Crap, I think we're hosed
                                    //
                                    ErrorMsg = string.Format("Oops - second try - can't process this file: {0}", ex2.Message);
                                }
                            }
                        }
                        

                        if (outputMp3Writer != null)
                        {
                            //
                            // If we're here, we've successfully created the MP3 converter from the WAV file, and our data stream
                            // is in the variable "waveDataToConvertToMp3"
                            //
                            try
                            {
                                byte[] buff = new byte[outputMp3Writer.OptimalBufferSize];
                                int read = 0;
                                while ((read = waveDataToConvertToMp3.Read(buff, 0, buff.Length)) > 0)
                                {
                                    outputMp3Writer.Write(buff, 0, read);
                                }

								//
							    // We have mp3 bytes, write 'em
							    //
							    // FileStream outputMp3File = new FileStream(targetMp3File, FileMode.CreateNew);
							    //outputMp3File.Write(originalSourceByteArray, 0, originalSourceByteArray.Length);
							    using (Stream outputMp3File = File.OpenWrite(targetMp3File))
                                {
                                    outputStream.Position = 0;
                                    Functions.CopyStream(outputStream, outputMp3File);
                                }

                                successfulWavConversionToMp3 = true;
                            }
                            catch (Exception ex)
                            {
                                ErrorMsg = string.Format("Oops, fatal error: {0}", ex.Message);
                            }
                            finally
                            {
                                outputMp3Writer.Close();
                                waveDataToConvertToMp3.Close();
                            }
                        }

                        if (!successfulWavConversionToMp3)
                        {
                            //
                            // Well, everthing failed.  We have a WAV at least, let's go ahead and write that.
                            //
                            File.WriteAllBytes(targetWavFile, originalSourceByteArray);
                        }

                        //
                        // Let's clean this stuff up
                        //
                        originalSourceWaveStream.Close();
                        
                        if (convertedSourceWaveStream != null)              
                        {
                            convertedSourceWaveStream.Close();
                        }
                    }
                    else
                    {
                        FileStream outputMp3File = null;
                        try
                        {
                            outputMp3File = new FileStream(targetMp3File, FileMode.CreateNew);
                            //
                            // We have mp3 bytes, write 'em
                            //
                            outputMp3File.Write(originalSourceByteArray, 0, originalSourceByteArray.Length);
                        }
                        catch
                        {
                            // Maybe we have the file already by another thread?
                            ErrorMsg = "Have mp3, can't write to disk.";
                        }
                        finally
                        {
                            if (outputMp3File != null)
                            {
                                outputMp3File.Close();
                            }
                        }

                        /*
                         * Huge todo: this code works fine on Windows 7, but doesn't work on Windows 2008 Server.  The problem is two fold:
                         * 
                         *  a) The two mp3 encoders installed by Win7 (l3codeca.acm and l3codecp.acm) don't exist.  I see no way to "install" them.
                         *      This site comes closes to explaining, but these reg keys do NOT exist so the solution doesn't work:
                         *      http://blog.komeil.com/2008/06/enabling-fraunhofer-mp3-codec-vista.html
                         *      
                         *  b) The alternate option explained here: 
                         *      http://stackoverflow.com/questions/5652388/naudio-error-nodriver-calling-acmformatsuggest/5659266#5659266
                         *      Also doesn't work since the COM object for the DMO isn't registered on Win2K8.  The Windows SDK is supposed to have it,
                         *      but it doesn't install properly on the server.  Also Win Media Player won't install with a weird "There is no update to Windows Media Player".
                         *      
                         *  I've googled for a long time, but this is a tricky one.  Maybe Microsoft needs to be contacted?
                         *  
                         * This is required to create WAV files for Firefox to play.  Otherwise you have to click on a link. (alt solution: embed?)

						NAudio.Wave.Mp3FileReader reader = null;
						try
						{
							//
							// Let's write the WAV bytes
							// http://hintdesk.com/c-mp3wav-converter-with-lamenaudio/
							// 
							using (reader = new NAudio.Wave.Mp3FileReader(targetMp3File))
							{
								using (NAudio.Wave.WaveStream pcmStream = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(reader))
								{
									NAudio.Wave.WaveFileWriter.CreateWaveFile(targetWavFile, pcmStream);
								}
							}
						}
						catch
						{
							ErrorMsg = "Have mp3, can't convert to WAV";
						}
						finally
						{
							if (reader != null)
							{
								reader.Close();
							}
						}

                        */

                    }
				}
                else
                {
                    ErrorMsg = "Cannot get sound id";
                }
            }
        }
        else
        {
            ErrorMsg = string.Format("Invalid request parameter: {0}", _encryptedPlayerRequest);
        }
    }
        
}