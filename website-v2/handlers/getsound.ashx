<%@ WebHandler Language="C#" Class="getsound" %>

using System;
using System.Web;
using System.IO;
using OttaMatta.Common;
using OttaMatta.Data;
using Yeti.MMedia;
using Yeti.MMedia.Mp3;
using NAudio;
using WaveLib;
using System.Diagnostics;
using OttaMatta.Application;

/// <summary>
/// Return a sound from the DB as an HTTP document.
/// </summary>
public class getsound : IHttpHandler {

    /// <summary>
    /// Make the magic happen.
    /// </summary>
    /// <param name="context">The request context</param>
    public void ProcessRequest (HttpContext context) {

        string encryptedPlayerRequest = context.Request["soundid"];
        string contentType = "text/plain";
        
        PlayerRequestInfo requestInfo = new PlayerRequestInfo(encryptedPlayerRequest);
        
        dynamic theSound = null;

        if (requestInfo.IsValid)   // Functions.IsNumeric(soundId))
        {
            int soundId = requestInfo.SoundId;
            
            //
            // First, look in the cache and see if we have a converted mp3 there already
            //
            string cacheDir = context.Server.MapPath(Config.CacheSoundsDirectory);
            string targetMp3File = Functions.CombineElementsWithDelimiter("\\", cacheDir, string.Format("id{0}.mp3", soundId));
            string targetWavFile = Functions.CombineElementsWithDelimiter("\\", cacheDir, string.Format("id{0}.wav", soundId));

            //
            // If there's an MP3 file already converted, let's use that.
            //
            if (File.Exists(targetMp3File))
            {
                contentType = "audio/mpeg"; //
                FileStream sourceMp3File = new FileStream(targetMp3File, FileMode.Open);
                sourceMp3File.CopyTo(context.Response.OutputStream);
                sourceMp3File.Close();
            }
            else
            {
                //
                // OK, let's grab the sound data from the DB
                //
                theSound = DataManager.GetSoundData(Functions.ConvertInt(soundId, -1));

                if (theSound != null && theSound.Id >= 0)
                {
                    //
                    // The DB returns HTML-ready bytes.  It would be more efficient to return the binary data.  Then we wouldn't have to do 2 conversions.
                    // Todo!
                    //
                    byte[] originalSourceByteArray = System.Convert.FromBase64String(theSound.Data);

                    if (Functions.IsWav(theSound.FileName))
                    {
                        //
                        // It's a WAV, gotta convert to MP3 since most browsers can only play a limited range of WAV bitrates.  They can all play
                        // MP3s though.
                        //
                        
                        Mp3Writer outputMp3Writer = null;
                        // Mp3Writer outputFileMp3Writer = null;
                        
                        WaveStream waveDataToConvertToMp3 = null;
                        WaveStream convertedSourceWaveStream = null;
                        WaveStream originalSourceWaveStream = new WaveStream(new MemoryStream(originalSourceByteArray));

                        try
                        {
                            outputMp3Writer = new Mp3Writer(context.Response.OutputStream, originalSourceWaveStream.Format);
                            waveDataToConvertToMp3 = originalSourceWaveStream;
                        }
                        catch         // (Exception ex)
                        {
                            //
                            // The source WAV isn't compatible with the LAME thingy.  Let's try to convert it to something we know is usable with the NAudio thingy.
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
                            NAudio.Wave.WaveFormatConversionStream str = new NAudio.Wave.WaveFormatConversionStream(targetFormat, stream);

                            //
                            // For lack of a better solution to get the bytes from the converted data into a "WaveStream" variable, use these
                            // available methods to write to a disk file and then open up the disk file.  The problem with directly converting
                            // with memory streams is that the required headers (like "RIFF" aren't written into the converted stream at this point.  
                            // 
                            NAudio.Wave.WaveFileWriter.CreateWaveFile(targetWavFile, str);
                            convertedSourceWaveStream = new WaveStream(targetWavFile);
                            
                            //
                            // Now we have a correct WAV memory stream
                            //
                            try
                            {
                                WaveFormat format = convertedSourceWaveStream.Format;
                                outputMp3Writer = new Mp3Writer(context.Response.OutputStream, format);
                                waveDataToConvertToMp3 = convertedSourceWaveStream;
                            }
                            catch (Exception ex2)
                            {
                                //
                                // Crap, I think we're hosed
                                //
                                context.Response.Write(string.Format("Oops - second try - can't process this file: {0}", ex2.Message));
                            }
                             
                            /* Doesn't work in mobile Safari:
                            contentType = "audio/wav";
                            context.Response.OutputStream.Write(newbytes, 0, newbytes.Length);
                            */
                        }
                        

                        if (outputMp3Writer != null)
                        {
                            //
                            // If we're here, we've successfully converted the WAV file into an MP3 file, and our data stream
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
                                contentType = "audio/mpeg"; //  MimeTypeFromFileName(theSound.FileName);

                                //
                                // Cache the results to disk so we don't have to do this again next time
                                // Todo: figure out how!
                                //
                                /*
                                using (Stream outputFile = File.OpenWrite(targetMp3File))
                                {
                                    // outputMp3Writer.BaseStream.CopyTo(outputFile);
                                    // context.Response.OutputStream.CopyTo(outputFile);
                                    
                                }
                                 * */
                            }
                            catch (Exception ex)
                            {
                                context.Response.Write(string.Format("Oops, fatal error: {0}", ex.Message));
                            }
                            finally
                            {
                                outputMp3Writer.Close();
                                waveDataToConvertToMp3.Close();
                            }
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
                        contentType = "audio/mpeg"; 
                        context.Response.OutputStream.Write(originalSourceByteArray, 0, originalSourceByteArray.Length);
                    }
                }
                else
                {
                }
            }
        }
        
        context.Response.ContentType = contentType;
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}