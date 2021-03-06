# Mensa Aachen App
A simple App for Canteens in Aachen. (WinPhone (not supported any more), iOS, Android, UWP)

## License
This Software uses the Newtonsoft json library, which is licensed under MIT-License (https://raw.githubusercontent.com/JamesNK/Newtonsoft.Json/master/LICENSE.md)

The Source Code of the Mensa Aachen App is licensed under the MIT-License.

## Frameworks/Dependencies
This project uses the Xamarin-Framework (Xamarin.iOS, Xamarin.Android) and the Windows Phone SDK.
It is a Shared Project, needing at least Visual Studio 2013 Update 2. Xamarin/WindowsPhone SDK may have further requirements.


## Projects
The solution has multiple projects:
* Mensa - Basic Regex and Data-Management (unused to to VS issues)
* MensaPortable - Regex and Data-Management (substitute for Mensa)
* MensaApp - Shared Project for UI, MVVM, Adapters and Localization Content
* MensaApp.WinPhone/Droid/iOS - The actual projects for the target mobile platforms, containing platform-specific code, Manifests, images
* MensaAppWin - A new shared project which is almost identical to MensaApp but also targets a UWP project
* MensaAppWin.UWP - The actual UWP project which targets Windows 10 devices (PC, Mobile, XBOX)
* MensaConsole - Simple Testing Console Application for the Importer
