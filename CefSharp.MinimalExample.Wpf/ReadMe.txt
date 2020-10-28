1- Run the Project and on Web Page, hit F12 to bring up Dev Tools
2- Go to Console and type window.open("http://www.google.com") - PopUp will open as per LifeSpanHandler (but it will go behind the DevTools, you need to bring in front)
3- Now again do F12 on the PopUp to bring up another DevTools --> Issue!! Title Shows DevTools, but, instead it loads the PopUp Url (google.com)
4- Close the "DevTools" you launched on Step 3, it will close the PopUp too, because Mainframe.Url is not DevTools URL, see Line 117

Help link: Cannot get DevTools instance, manual hack - https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=12119