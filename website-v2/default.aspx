<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_default" %>
<%@ Import Namespace="OttaMatta.Common" %>
<%@ Register TagPrefix="Otm" Src="~/Controls/analytics.ascx" TagName="Analytics" %>

<%-- 

Otamata Website - Todos

* Implement "no-cache" pragma headers so that form can't be cached by the browser (help prevent double submit)
* Add more RAM to hosted server
* Get this url to work: http://otamata.com/
* Add version number to content dirs (like Fandango) so that browser cache clearing is easier (prolly adding a url rewrite)
* Start creating unit/integration tests
* Player: add NSFW message

Maybe

* Textbox for link to more info about the item (e.g. link to movies.com).  
* Allow for cropping of an image.  Use the yahoo cropper: http://developer.yahoo.com/yui/imagecropper/
* Allow any size image, and resize/crop on the server: http://imagemagick.codeplex.com/

Done

* Change doctype to be HTML5
* Finish up legal language
* Update stored procs to record uuid and program version
* Create google analytics account for otamata
* Update app icon - verify all colors: http://colorschemedesigner.com/#2Y61Aw0w0w0w0
* Add fav icon
* Add language about obscene, porn, racist, hate content



--%>


<!DOCTYPE html>
<html lang="en">
<head>
	<title><%=FullAppName %></title>
	
	<meta charset="utf-8" />
    <meta name="description" content="<%=FullAppName%> is the simplest way to share sound clips and
                sound effects with your friends. Tweet out sounds, post them to
                Facebook or annoy your friends by playing them on your phone in
                totally inappropriate situations. Ironic slow clap, included." />

	<meta name="viewport" content="width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=no" />
	
	<!-- Stylesheets -->
	<link rel="stylesheet" type="text/css" href="stylesheets/base.css" />
	<link rel="stylesheet" type="text/css" href="stylesheets/media.queries.css" />
	<link rel="stylesheet" type="text/css" href="stylesheets/tipsy.css" />
	<link rel="stylesheet" type="text/css" href="javascripts/fancybox/jquery.fancybox-1.3.4.css" />
	<link rel="stylesheet" type="text/css" href="http://fonts.googleapis.com/css?family=Nothing+You+Could+Do|Quicksand:400,700,300">
	
	<!-- Favicons -->
	<link rel="shortcut icon" href="images/icon-16x16.png" />
	<link rel="apple-touch-icon" href="images/apple-touch-icon.png">
	<link rel="apple-touch-icon" sizes="72x72" href="images/apple-touch-icon-72x72.png">
	<link rel="apple-touch-icon" sizes="114x114" href="images/apple-touch-icon-114x114.png">
	
</head>
<body>
	<!-- Start Wrapper -->
	<div id="page_wrapper">
		
	<!-- Start Header -->
	<header>
		<div class="container">
			<!-- Start Social Icons -->
			<aside>
				<ul class="social">
					<li class="facebook"><a href="http://www.facebook.com/otamatasndboard">Facebook</a></li>
					<li class="twitter"><a href="http://twitter.com/otamatasndboard">Twitter</a></li>
					<li class="email"><a href="mailto:general@otamata.com" title="general@otamata.com">Email</a></li>
					
                    <%-- More Social Icons:
					<li class="rss"><a href="" title="App Updates">RSS</a></li>
					<li class="dribbble"><a href="">Dribbble</a></li>
					<li class="google"><a href="">Google</a></li>
					<li class="flickr"><a href="">Flickr</a></li>
					--%>
				</ul>
			</aside>
			<!-- End Social Icons -->
			
			<!-- Start Navigation -->
			<nav>
				<ul>
					<li><a href="#home">Home</a></li>
					<li><a href="#upload">Upload</a></li>
					<li><a href="#features">Features</a></li>
					<li><a href="#updates">Updates</a></li>
					<li><a href="#about">About Us</a></li>

					<%--  ** Stuff available to put in later as necessary
					<li><a href="#team">Team</a></li>
					<li><a href="#screenshots">Screenshots</a></li>
                    <li><a href="#press">Press</a></li>
					<li><a href="#contact">Contact</a></li>

                    <!-- This page shows a bunch of other cool stuff in the template -->
					<li><a href="#styles">Styles</a></li> 
                    --%>
				</ul>
				<span class="arrow"></span>
			</nav>
			<!-- End Navigation -->
		</div>
	</header>
	<!-- End Header -->
	
	<section class="container">
		
		<!-- Start App Info -->
		<div id="app_info">
			<!-- Start Logo -->
			<a href="#home" class="logo">
				<img src="images/otamata/logo-otamata-290x56.png" alt="Otamata" />
				<%--<img src="images/otamata/logo-290x70.jpg" alt="Otamata" />--%>
				<%--<img src="images/light-logo.png" alt="Otamata" />--%>
			</a>
			<!-- End Logo -->
			<span class="tagline">The Social Soundboard</span>
			<p>
				<%=FullAppName%> is the simplest way to share sound clips and
                sound effects with your friends. Tweet out sounds, post them to
                Facebook or annoy your friends by playing them on your phone in
                totally inappropriate situations. Ironic slow clap, included.
			</p>
			
			<div class="buttons">
				<a href="<%=iTunesStoreUrl %>" class="large_button" id="apple">
					<span class="icon"></span>
					<em>Download now for</em> iPhone
				</a>
                <%-- 
				<a href="#" class="large_button" id="android">
					<span class="icon"></span>
					<em>Download now for</em> Android
				</a>
                --%>
			</div>
			
			<div class="price centered"> <!-- Alignments options: right_align, left_align, centered -->
				<p>Free in the iTunes Store for a limited time!</p>
			</div>
		</div>
		<!-- End App Info -->		
		
		<!-- Start Pages -->
		<div id="pages">
			<div class="top_shadow"></div>
			
			<!-- Start Home -->
			<div id="home" class="page">
				
				<div id="slider">
                    <%-- Make sure to have pairs of screenshots.  If the last one is a single, the following transition puts the two shots on top of each other --%>
					<div class="slide" data-effect-out="slide">
						
						<div class="background iphone-black">
							<img src="images/slider/screen_2.jpg" alt="" />
						</div>
						
						<div class="foreground iphone-black">
							<img src="images/slider/screen_1.jpg" alt="" />
						</div>
						
					</div>
					
					<div class="slide" data-effect-in="slide">
						
						<div class="background iphone-black">
							<img src="images/slider/screen_4.jpg" alt="" />
						</div>
						
						<div class="foreground iphone-black">
							<img src="images/slider/screen_3.jpg" alt="Search for new sounds">
						</div>
						
					</div>
				
					<div class="slide">
						
						<div class="background iphone-black">
							<img src="images/slider/screen_6.jpg" alt="" />
						</div>
						
						<div class="foreground iphone-black">
							<img src="images/slider/screen_5.jpg" alt="" />
						</div>
						
					</div>
					
                        <%-- 
					<div class="slide">
						
						<div class="background iphone-black">
							<img src="images/slider/credits.jpg" alt="" />
						</div>
						
						<div class="foreground iphone-white">
							<img src="images/slider/android-back.jpg" alt="" />
						</div>
					</div>
						--%>
					<%--
					<div class="slide">
						
						<div class="background ipad-black">
							<iframe src="http://player.vimeo.com/video/40603475?title=0&amp;byline=0&amp;portrait=0" width="352" height="468" frameborder="0"></iframe>
						</div>
												
					</div>

					<div class="slide">
						
						<div class="background ipad-white">
							<img src="images/slider/ipad.jpg" alt="" />
						</div>
												
					</div>
					--%>
				</div>
			
			</div>
			<!-- End Home -->
			
            <!-- Start upload -->
            <div id="upload" class="page">
                <iframe id="uploadIFrame" src="upload-iframe.aspx?x=1" frameborder="0" height="575px" ></iframe> <%-- Note: width is set by javascript in fluidapp.js - "resizePage()" --%>

            </div>
            <!-- End upload-->

            <%-- ** Team members page if necessary someday!
			<!-- Start Team -->
			<div id="team" class="page">
				
				<h1>Team</h1>
				
				<div class="about_us content_box">
					<div class="one_half">
						<h2>About Us</h2>
						<p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Phasellus hendrerit. Pellentesque aliquet nibh nec urna. In nisi neque, aliquet vel, dapibus id, mattis vel, nisi. Sed pretium, ligula sollicitudin laoreet viverra, tortor libero sodales leo, eget blandit nunc tortor eu nibh. Nullam mollis. Ut justo.</p>
					</div>

					<div class="one_half column_last">
						<img src="images/about-main.png" alt="" />
					</div>
				</div>
				
				<div class="team_members">
					<div class="person one_half">
						<img src="images/about-team_1.png" alt="" />
						<h3>Jane Doe</h3>
						<span>Designer</span>
						<a href="#">http://website.com</a>
						<ul class="social">
							<li class="facebook"><a href="">Facebook</a></li>
							<li class="twitter"><a href="">Twitter</a></li>
							<li class="dribbble"><a href="">Dribbble</a></li>							
						</ul>
					</div>
					<div class="person one_half column_last">
						<img src="images/about-team_2.png" alt="" />
						<h3>John Smith</h3>
						<span>Developer</span>
						<a href="#">http://website.com</a>
						<ul class="social">
							<li class="facebook"><a href="">Facebook</a></li>
							<li class="twitter"><a href="">Twitter</a></li>
							<li class="dribbble"><a href="">Dribbble</a></li>							
						</ul>
					</div>
					<div class="person one_half">
						<img src="images/about-team_3.png" alt="" />
						<h3>John Doe</h3>
						<span>UI/UX Expert</span>
						<a href="#">http://website.com</a>
						<ul class="social">
							<li class="facebook"><a href="">Facebook</a></li>
							<li class="twitter"><a href="">Twitter</a></li>
							<li class="dribbble"><a href="">Dribbble</a></li>							
						</ul>
					</div>
					<div class="person one_half column_last">
						<img src="images/about-team_4.png" alt="" />
						<h3>Mary Smith</h3>
						<span>Support</span>
						<a href="#">http://website.com</a>
						<ul class="social">
							<li class="facebook"><a href="">Facebook</a></li>
							<li class="twitter"><a href="">Twitter</a></li>
							<li class="dribbble"><a href="">Dribbble</a></li>							
						</ul>
					</div>
				</div>
				
			</div>
			<!-- End Team -->
            --%>
				
			<!-- Start Features -->
			<div id="features" class="page">
				
				<h1>Features</h1>
				
				<div class="feature_list content_box">
					<div class="one_half">
						<h2 class="icon speaker">Social Sharing</h2>
						<p>Ease the tension with your girlfriend by texting her the sound of a screeching cat,
                        or make fun of your boss with a sound clip from your favorite movie. With social
                        sharing, you can instantly post sounds via Facebook, Twitter, SMS or email for cheap
                        laughs and easy thrills.</p>
					</div>

					<div class="one_half column_last">
                        <%--
						<h2 class="icon speaker">Sound Market</h2>
						<p>Download up to 10 sounds for free from our ever-growing sound market or upload a sound of your 
                        choice from the web. Super soundboarders can purchase unlimited downloads for <%=UnlimitedDLPrice %>.</p>
                        --%>

                        <h2 class="icon speaker">Recorder</h2>
						<p>The Recorder makes it easy to capture all kinds of moments in
                        real-time:  your baby’s first words, the crowd at a sports arena or a
                        rare diatribe from your inebriated best friend. Name it, save it and
                        BOOM—share it.</p>
					</div>

					<div class="one_half">
						<h2 class="icon speaker">One-Touch Quick Play</h2>
						<p>In comedy, they say timing is everything. Lost opportunities in humor will be a distant
                        memory with the easy-to-navigate soundboard. Once you launch the app, you’re
                        one tap away from playing your sound.</p>
					</div>
                    
					<div class="one_half column_last">
						<h2 class="icon speaker">Sound Search</h2>
						<p>Browse sounds by keyword, rating or download date—either way, you can easily find the clips you want with the minimum amount of effort.</p>
					</div>

                    <%-- It's good to have the home page be the longest.  Extra length can cause a scrollbar and weird horizonal jumping during transitions.
					<div class="one_half">
						<h2 class="icon speaker">Volume Control</h2>
						<p>You can choose how loud or how quiet you want your sounds. Automatically set the volume on startup, and play sounds even if your mute switch is on.</p>
					</div>

					<div class="one_half column_last">
						<h2 class="icon speaker">Community Ratings</h2>
						<p>We are a soundboard of the people, by the people and for the people. As a member
                        of the community, you decide which sounds rock, which sounds suck and which
                        sounds need a NSFW tag.</p>
					</div>
                    --%>
				</div>
				
			</div>
			<!-- End Features -->		
			
            <%-- ** This page is kind of redundant with the homepage.  Note: screenshot graphics load with the page, making
            the total page size very heavy.  If uncommenting this, consider lazy loading the images when the user clicks to
            load the page.
			<!-- Start Screenshots -->
			<div id="screenshots" class="page">
				
				<h1>Screenshots</h1>
				
				<p>Vivamus sagittis lacus vel augue laoreet rutrum faucibus dolor auctor.</p>
				
				<div class="screenshot_grid content_box">
					
					<div class="one_third">
						<a href="images/screenshots/screen_1.jpg" class="fancybox" rel="group" title="Screenshot 1"><img src="images/screenshots/screen_1.jpg" alt="" /></a>
					</div>

					<div class="one_third">
						<a href="images/screenshots/screen_2.jpg" class="fancybox" rel="group" title="Screenshot 2"><img src="images/screenshots/screen_2.jpg" alt="" /></a>
					</div>
					
					<div class="one_third column_last">
						<a href="images/screenshots/screen_3.jpg" class="fancybox" rel="group" title="Screenshot 3"><img src="images/screenshots/screen_3.jpg" alt="" /></a>
					</div>
					
					<div class="one_third">
						<a href="images/screenshots/screen_4.jpg" class="fancybox" rel="group" title="Screenshot 4"><img src="images/screenshots/screen_4.jpg" alt="" /></a>
					</div>

					<div class="one_third">
						<a href="images/screenshots/screen_5.jpg" class="fancybox" rel="group" title="Screenshot 5"><img src="images/screenshots/screen_5.jpg" alt="" /></a>
					</div>
					
					<div class="one_third column_last">
						<a href="images/screenshots/screen_6.jpg" class="fancybox" rel="group" title="Screenshot 6"><img src="images/screenshots/screen_6.jpg" alt="" /></a>
					</div>
					
					<div class="one_third">
						<a href="images/screenshots/screen_7.jpg" class="fancybox" rel="group" title="Screenshot 7"><img src="images/screenshots/screen_7.jpg" alt="" /></a>
					</div>

					<div class="one_third">
						<a href="images/screenshots/screen_8.jpg" class="fancybox" rel="group" title="Screenshot 8"><img src="images/screenshots/screen_8.jpg" alt="" /></a>
					</div>
					
					<div class="one_third column_last">
						<a href="images/screenshots/screen_9.jpg" class="fancybox" rel="group" title="Screenshot 9"><img src="images/screenshots/screen_9.jpg" alt="" /></a>
					</div>
					
				</div>
				
			</div>
			<!-- End Screenshots -->
			--%>

			<!-- Start Updates -->
			<div id="updates" class="page">
				
				<h1>Updates</h1>
				
				<div class="releases">
					<article class="release">
						<h2>Version 1.4</h2>
						<span class="date">Special release on March 31, 2013</span>
						<ul>
							<li class="new">Apple is no longer allowing updates to this app.  However, the most recent update is quite good and it's a shame to let it 
                            go to waste.  So, those of you with jailbroken phones can <a href="/downloads/Otamata-1.4.48.6.ipa">download the file and install it</a>.  The 
                            additions are as follows:</li>
							<li class="new"><span><b>New</b></span> Added web search feature - search internet for sounds and icons</li>
							<li class="new"><span><b>New</b></span> Icon editor - after searching for an icon image, move and crop it before adding it to your sound</li>
							<li class="new"><span><b>New</b></span> Added new "Share" option - "Preview in Safari"</li>
                            <li class="new"><span><b>New</b></span> Removed download limitation - everyone now has unlimited downloads</li>
						</ul>
					</article>
					
					<article class="release">
						<h2>Version 1.3</h2>
						<span class="date">Released on June 29, 2012</span>
						<ul>
							<li class="new"><span><b>New</b></span> Added built-in sound recorder</li>
							<li class="fix"><span><b>Fix</b></span> Usability enhancements</li>
						</ul>
					</article>
					
					<article class="release">
						<h2>Version 1.2</h2>
						<span class="date">Released on May 25, 2012</span>
						<ul>
							<li class="new"><span><b>New</b></span> Added support for sharing sounds on Facebook, Twitter, and email</li>
							<li class="new"><span><b>New</b></span> Added ability to share the default sounds, not just the user uploaded ones</li>
						</ul>
					</article>
					
					<article class="release">
						<h2>Version 1.1</h2>
						<span class="date">Released on May 2, 2012</span>
						<ul>
							<li class="new"><span><b>New</b></span> Annoy friends from a distance: send sounds to friends via TEXT MESSAGING</li>
							<li class="new"><span><b>New</b></span> Added a more helpful "Welcome" tutorial.  The original wasn't bad, but now it's better</li>
							<%--<li class="fix"><span><b>fix</b></span> Various UI enhancements</li> --%>
						</ul>
					</article>
					
					<article class="release">
						<h2>Version 1.0</h2>
						<span class="date">Released on March 5, 2012</span>
						<ul>
							<li class="new"><span><b>New</b></span> Initial release for iOS</li>
						</ul>
					</article>
				</div>
				
			</div>
			<!-- End Updates -->
					
            <%-- It would be cool if We needed this someday
			<!-- Start Press -->
			<div id="press" class="page">
				
				<h1>Press</h1>
				
				<div class="press_mentions">
					<ul>
						<li>
							<div class="logo">
								<img src="images/light-press.png" alt="" />
							</div>
							<div class="details">
								<p>"The best mobile app website you’ve ever seen!"</p>
								<address>
									Jane Doe
									<a href="#">http://website.com &#x2192;</a>
								</address>
							</div>
						</li>	
						<li>
							<div class="logo">
								<img src="images/light-press.png" alt="" />
							</div>
							<div class="details">
								<p>"Cras mattis consectetur purus sit amet fermentum."</p>
								<address>
									Jane Doe
									<a href="#">http://website.com &#x2192;</a>
								</address>
							</div>
						</li>
						<li>
							<div class="logo">
								<img src="images/light-press.png" alt="" />
							</div>
							<div class="details">
								<p>"Etiam porta sem malesuada magna mollis euismod. Nullam quis risus eget urna mollis ornare vel eu leo."</p>
								<address>
									Jane Doe
									<a href="#">http://website.com &#x2192;</a>
								</address>
							</div>
						</li>
						<li>
							<div class="logo">
								<img src="images/light-press.png" alt="" />
							</div>
							<div class="details">
								<p>"Vivamus sagittis vel augue rutrum faucibus dolor."</p>
								<address>
									Jane Doe
									<a href="#">http://website.com &#x2192;</a>
								</address>
							</div>
						</li>
						<li>
							<div class="logo">
								<img src="images/light-press.png" alt="" />
							</div>
							<div class="details">
								<p>"Maecenas faucibus mollis interdum."</p>
								<address>
									Jane Doe
									<a href="#">http://website.com &#x2192;</a>
								</address>
							</div>
						</li>
					</ul>
				</div>
				
			</div>
			<!-- End Press -->
			--%>

            <!-- Start About Us -->
			<div id="about" class="page">
				
				<h1>About Us</h1>
				
				<p>Otamata-whatta?</p>

                <p>Otamata got its name from onomatopoeia, which is a word that imitates
                or suggests the source of the sound that it describes. Basically,
                we’re the boom, bang, pow of social soundboarding.
                </p>

                <p>Our goal is to put the world of sound in the palm of your hand with a
                simple, easy-to-use format. Starting with the launch of our first
                iPhone app, Otamata is one of the first FREE all-in-one mobile sound
                applications that enables you to upload your own sounds, download
                sounds from other users, record sounds and share sounds via Facebook,
                Twitter, email or text — all within minutes of downloading (no training
                manuals or tech experience required). Our first app is available now
                for download at the <a href="<%=iTunesStoreUrl %>">iTunes Store</a>, with an Android version
                coming soon.</p>
            </div>
            <!-- End About Ua -->

            <%-- Eh, no PHP on the server and a mailto: link will suffice for now.  Let's keep it simple.
			<!-- Start Start Contact -->
			<div id="contact" class="page">
				
				<h1>Contact</h1>
				
				<p>For general questions, bug reports or press inquires please fill out the form below.</p>
				
				<div id="contact_form">
					
					<div class="validation">
						<p>Oops! Please correct the highlighted fields...</p>
					</div>

					<div class="success">
						<p>Thanks! I'll get back to you shortly.</p>
					</div>
				
					<form action="javascript:;" method="post">
						<div class="row">
							<p class="left">
								<label for="name">Name*</label>
								<input type="text" name="name" id="name" value="" />
							</p>
							<p class="right">
								<label for="email">Email*</label>
								<input type="text" name="email" id="email" value="" />
							</p>
						</div>
					
						<div class="row">
							<p class="left">
								<label for="website">Website</label>
								<input type="text" name="website" id="website" value="" />
							</p>
							<p class="right">
								<label for="subject">Subject</label>
								<input type="text" name="subject" id="subject" value="" />
							</p>
						</div>
					
						<p>
							<label for="message" class="textarea">Message</label>
							<textarea class="text" name="message" id="message"></textarea>
						</p>
					
						<input type="submit" class="button white" value="Send &#x2192;" />
					</form>
				
				</div>
				
			</div>
			<!-- End Start Contact -->
			--%>

            <%-- This is a demo page of the different styles.  Kept here for nostalgia; use the original HTML as the reference.
			<!-- Start Styles -->
			<div id="styles" class="page">
				
				<h1>Styles</h1>
				
				<div class="full">
					<h1>h1. Nullam id dolor id nibh ultricies.</h1>
					<h2>h2. Nullam id dolor id nibh ultricies.</h2>
					<h3>h3. Nullam id dolor id nibh ultricies.</h3>
					<h4>h4. Nullam id dolor id nibh ultricies.</h4>
					<h5>h5. Nullam id dolor id nibh ultricies.</h5>
					<h6>h6. Nullam id dolor id nibh ultricies.</h6>
				</div>
				
				<h2>Blockquotes</h2>
				
				<div class="one_half">
					<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.</p>
				</div>

				<div class="one_half column_last">
					<blockquote>
					  <p>This is a blockquote style example. It's cool.</p>
					  <cite>Some Dude, Some Website</cite>
					</blockquote>
				</div>
				
				<div class="full">
				
					<h2>Small Buttons</h2>
				
					<a href="#" class="button black">Black</a>
					<a href="#" class="button white">White</a>
					<a href="#" class="button gray">Gray</a>
					<a href="#" class="button orange">Orange</a>
					<a href="#" class="button blue">Blue</a>
					<a href="#" class="button green">Green</a>
					<a href="#" class="button pink">Pink</a>			
					<a href="#" class="button purple">Purple</a>
				
				</div>
				
				<div class="full">
					
					<h2>Large Buttons</h2>

					<a href="#" class="large_button" id="apple">
						<span class="icon"></span>
						<em>Download now for</em> iPhone
					</a>
					<a href="#" class="large_button" id="android">
						<span class="icon"></span>
						<em>Download now for</em> Android
					</a>
					<a href="#" class="large_button" id="windows">
						<span class="icon"></span>
						<em>Download now for</em> Windows
					</a>
					<a href="#" class="large_button" id="blackberry">
						<span class="icon"></span>
						<em>Download now for</em> Blackberry
					</a>
					
				</div>
				
				<h2>Columns</h2>
				
				<div class="one_half">
					<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sed diam eget risus varius blandit sit amet non magna. Morbi leo risus, porta ac consectetur ac, vestibulum at eros.</p>
				</div>
				
				<div class="one_half column_last">
					<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sed diam eget risus varius blandit sit amet non magna. Morbi leo risus, porta ac consectetur ac, vestibulum at eros.</p>
				</div>
				
				<div class="one_third">
					<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sed diam eget risus varius blandit sit amet non magna.</p>
				</div>
				
				<div class="one_third">
					<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sed diam eget risus varius blandit sit amet non magna.</p>
				</div>
				
				<div class="one_third column_last">
					<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas sed diam eget risus varius blandit sit amet non magna.</p>
				</div>
				
				<div class="one_third">
					<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>
				</div>
				
				<div class="two_thirds column_last">
					<p>Duis mollis, est non commodo luctus, nisi erat porttitor ligula, eget lacinia odio sem nec elit. Maecenas faucibus mollis interdum. Morbi leo risus, porta ac consectetur ac, vestibulum at eros. Curabitur blandit tempus porttitor. Donec sed odio dui. Morbi leo risus, porta ac consectetur ac, vestibulum.</p>
				</div>
				
				<h2>Toggle Lists</h2>
				
				<div class="toggle_list">
					<ul>
						<li class="opened"> <!-- Use class "opened" to expand a toggle on page load -->
							<div class="title">
								<h3><span>Q.</span> What are the requirements for using this app?</h3>
								<a href="javascript:;" class="toggle_link" data-open_text="+" data-close_text="-"></a>
							</div>
							<div class="content">
								<p>Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>

								<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus sagittis lacus vel augue laoreet rutrum faucibus dolor auctor.</p>
							</div>
						</li>
						<li>
							<div class="title">
								<h3><span>Q.</span> How does it work?</h3>
								<a href="javascript:;" class="toggle_link" data-open_text="+" data-close_text="-"></a>
							</div>
							<div class="content">
								<p>Donec ullamcorper nulla non metus auctor fringilla. Maecenas sed diam eget risus varius blandit sit amet non magna. Morbi leo risus, porta ac consectetur ac, vestibulum at eros.</p>
							</div>
						</li>
						<li>
							<div class="title">
								<h3><span>Q.</span> How much does it cost?</h3>
								<a href="javascript:;" class="toggle_link" data-open_text="+" data-close_text="-"></a>
							</div>
							<div class="content">
								<p>Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>

								<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus sagittis lacus vel augue laoreet rutrum faucibus dolor auctor.</p>
							</div>
						</li>
					</ul>
				</div>
				
				<h2>Lightbox Images</h2>
				
				<div class="one_third">
					<a href="images/screenshots/screen_1.jpg" rel="screenshots" class="fancybox" title=""><img src="images/screenshots/screen_1.jpg" alt="" /></a>
				</div>
				<div class="one_third">
					<a href="images/screenshots/screen_2.jpg" rel="screenshots" class="fancybox" title=""><img src="images/screenshots/screen_2.jpg" alt="" /></a>
				</div>
				<div class="one_third column_last">
					<a href="images/screenshots/screen_3.jpg" rel="screenshots" class="fancybox" title=""><img src="images/screenshots/screen_3.jpg" alt="" /></a>
				</div>
				
				<div class="full">
				
					<h2>Tooltips</h2>
				
					<p>Cras justo odio, dapibus ac <a href="javascript:;" rel="tipsy" title="Example Tooltip">facilisis</a> in, egestas eget quam. Donec ullamcorper nulla non metus auctor fringilla. Nullam quis risus eget urna <a href="javascript:;" rel="tipsy" title="An even longer tooltip! <br/> With more stuff!">mollis ornare</a> vel eu leo.</p>
				
				</div>
				
			</div>
			<!-- End Styles -->
			--%>


			<div class="bottom_shadow"></div>
		</div>
		<!-- End Pages -->
		
		<div class="clear"></div>
	</section>
	
	<!-- Start Footer -->
	<footer class="container">
		<p>Copyright &copy; 2012 Ergocentric Software, Inc. All Rights Reserved.</p>
        <Otm:Analytics ID="AnalyticsControl" runat="server" />
	</footer>
	<!-- End Footer -->
	
	</div>
	<!-- End Wrapper -->


    <!-- Javascripts -->
	<%--<script type="text/javascript" src="javascripts/jquery-1.7.1.min.js"></script>--%>

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <script>        window.jQuery || document.write('<script src="javascripts/jquery-1.7.1.min.js"><\/script>')</script>

	<script type="text/javascript" src="javascripts/html5shiv.js"></script>
	<script type="text/javascript" src="javascripts/jquery.tipsy.js"></script>
	<script type="text/javascript" src="javascripts/fancybox/jquery.fancybox-1.3.4.pack.js"></script>
	<script type="text/javascript" src="javascripts/fancybox/jquery.easing-1.3.pack.js"></script>
	<script type="text/javascript" src="javascripts/jquery.touchSwipe.js"></script>
	<script type="text/javascript" src="javascripts/jquery.mobilemenu.js"></script>
	<script type="text/javascript" src="javascripts/jquery.infieldlabel.js"></script>
	<script type="text/javascript" src="javascripts/jquery.echoslider.js"></script>
	<script type="text/javascript" src="javascripts/fluidapp.js"></script>
	

    <%-- ASP.NET Webform stuff.  Let's convert this to ASP.NET MVC soon to get rid of this page lifecycle sillyness. --%>
    <form id="form1" runat="server">
    </form>
</body>
</html>

