# Duolingo Bot

A bot that does the Duolingo tree

- Written for fun, and because I'd never written a bot. It's arguably the stupidest bit of code I've ever written.
- Will automatically work through the Duolingo tree doing lessons and stories, but can be made to just do endless practice sessions
- Please don't use it to cheat: cheating on Duolingo is stupid.  It's not even a very good cheat, because a faster way to get points on Duolingo is to do the three-and-out 'jump here' tests (50XP), or the legendary challenges (40XP), and the bot can't do these.
- Written in C# .NET 7 and Selenium 4.8 talking to Chrome on the desktop.
- Written in an old-school OO plugin style.  It even has [template methods](https://en.wikipedia.org/wiki/Template_method_pattern).
- The bot is intended for Windows, and is best with Visual Studio.  However, it has also been tested on MacOS with a built version of the code, where it works fine.

## Instructions

- Prerequisites for these instructions are Windows, Chrome, and Visual Studio 2022.
- Download a [WebDriver for Chrome](https://chromedriver.chromium.org/downloads) for your [version of Chrome](https://www.google.com/intl/en_uk/chrome/update/).
- Unzip it as C:\Program Files(x86)\chromedriver_win32\chromedriver.exe.  You can also put it somewhere else and point to that location with the chromedriver setting in Settings.txt.
- Load DuolingoBotCS.sln in [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/) and hit F5.  It should fire up Chrome and show the Duolingo login screen.  Log in.
- The code will break in a method called DoTree() in Program.cs.  If Duolingo has loaded correctly and is showing the main tree you should be able to hit F5, continue, at this point and it should do a lesson before breaking here again.  We're only breaking to give you some control here: you can comment out the Debugger.Break statement if you want the bot to just run.
- To switch to practice mode change the setting in Settings.txt and restart the bot.
- The bot uses text in the base language to identify screen items.  The base language is the one you are using as your native language in the current course you are running.  That is, if your continue button when you are doing exercises says 'CONTINUE' then your base language is English, if it says 'CONTINUAR' you're probably in Spanish. If your base language is not English the bot won't work out the box.  You need to change the baselanguage setting in Settings.txt.  At present only français and español are set up though.  See below.
- You can also set up the bot to autologin. Just put your username and password in the appropriate settings in Settings.txt. 

## Issues

- **This is a screenscraper, so it will probably break as soon as Duolingo change the format of any of their web pages.**
- If you crash out then Duo may not let you log in again immediately.  If you log out manually using the Duo menus when broken on the breakpoint in DoTree, and then shut down the bot, then usually you can restart successfully.
- If you break on an error in debug, just hit continue, maybe a few times: usually the program will recover and continue.
- If you want to set up another base language this is some work: you need to set up BaseLanguage.cs for your language in the same way as French and Spanish.  The various settings are described in BaseLanguage.cs.
- Settings.txt is just a standard text file set to 'Copy if newer' to the build folder on a build.  If you want to change a setting either change it in the build folder, or change some code (just add a blank line somewhere in a .cs file) and rebuild.  Otherwise it may not copy and your new settings may not be picked up.
- As usual with Duolingo things work faster if you turn off Animations and Motivational Messages in More/Settings.  You can do this from the breakpoint in DoTree.

