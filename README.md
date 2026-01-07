# ExeLauncher

This is a local-only Windows file launcher (WPF) that opens files and URIs on your machine.

Files added and their purpose:
- `App.xaml` / `App.xaml.cs`: WPF application entry.
- `MainWindow.xaml` / `MainWindow.xaml.cs`: Main UI and code-behind.
- `ViewModels/MainViewModel.cs`: MVVM viewmodel with commands and recent files.
- `ViewModels/Helpers/RelayCommand.cs`: Simple ICommand implementation.
- `Models/SettingsModel.cs`: Settings container serialized as JSON.
- `Services/SettingsManager.cs`: Loads/saves `settings.json` in `%APPDATA%\ExeLauncher`.
- `Services/FileHandler.cs`: Core logic to handle .exe, .zip, media files, and URIs.
- `Services/ZipHandler.cs`: Extracts `.zip` to the configured folder and opens it.
- `Services/UrlHandler.cs`: Opens allowed URL schemes (`discord`, `roblox`, `http`, `https`).

Security and usage notes:
- The app will never auto-run files. The user must click "Run File" to execute.
- The app never downloads or runs remote code; it only opens local files or registered URI handlers.
- Allowed URI schemes are limited to common, safe ones. You can update `UrlHandler` to add other schemes.

Build & run:
Use the .NET SDK (6/7/8). From the project folder run:

```powershell
dotnet build
dotnet run
```

Publishing (portable single-file)
--------------------------------
To create a portable, single-file, self-contained Windows executable (no install required), run from the project root:

```powershell
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=false --self-contained true -o publish\win-x64
```

This produces a single EXE in `publish\win-x64` you can copy to another Windows machine and run without installing the SDK. For 32-bit, use `-r win-x86`.

Publishing to GitHub
--------------------
1. Create a new GitHub repository and push this project.
2. In your repository's Releases page, create a new release and upload the produced single-file EXE from the `publish` folder.
3. Users can download that release asset and run the EXE locally (no installer required).

Notes & constraints
-------------------
- The app is Windows-only (it launches `.exe` files and relies on Windows shell behavior).
- The app never downloads or runs remote code. Publishing to GitHub simply distributes the compiled binary; users must still download it before running.

Drag-and-drop usage
-------------------
- You can drag an `.exe` file onto the app window; only `.exe` is accepted via drag-and-drop. The `Run File` button will become enabled—click it to run the selected program.

Importing a prepared workspace (clone-and-run)
----------------------------------------------
You can prepare a repository with example files (a `workspace` folder) and let others clone it and import those files into the app workspace when they run the app.

Examples:

- Clone the repo and run the app importing the local `workspace` folder:

```powershell
git clone https://github.com/yourusername/yourrepo.git
cd yourrepo
dotnet run -- ./workspace
```

- Run the published EXE and import files from a folder:

```powershell
ExeLauncher.exe "C:\path\to\folder-with-files"
```

Behavior & security:
- The app will copy allowed file types into the local workspace and list them in the simulated desktop. It will NOT automatically execute any files on import — users must click `Run File` or click an item and press `Run` to execute.
- Allowed file types: `.exe`, `.zip`, `.txt`, `.png`, `.jpg`, `.jpeg`, `.mp3`, `.mp4` (configurable in `MainViewModel.AddFilesToWorkspace`).

Web deployment for wolfattack.us (browser-only launcher)
-----------------------------------------------------
I added a browser-only web version under the `web/` folder. It supports drag-and-drop, previews, and client-side ZIP extraction (JSZip). Important limits:

- Browsers cannot execute `.exe` files. The web launcher can show `.exe` tiles and allow users to download them, but it cannot run them. To open apps like Discord/Roblox/Spotify from the browser, use protocol links (e.g., `discord://`, `roblox://`, `spotify:`) — clicking such links will prompt the OS to open the associated app if installed.

How to publish this web app to GitHub Pages and use your domain `wolfattack.us`:

1. Create a GitHub repo and push this project (include the `web/` folder).
2. In the repository settings -> Pages, set the source to the `web/` folder on the `main` branch (or choose `gh-pages` branch or `docs/`).
3. Add a `CNAME` file containing `wolfattack.us` at the root of the publishing folder (I added `web/CNAME`).
4. On Squarespace (where you bought the domain), go to Domain settings and add DNS records to point the domain to GitHub Pages. For the apex domain, add these A records:

```
185.199.108.153
185.199.109.153
185.199.110.153
185.199.111.153
```

Alternatively, create a CNAME for `www` to `yourusername.github.io` and set up forwarding from apex to `www` in Squarespace.

5. Wait for DNS propagation and confirm the site loads at `https://wolfattack.us`.

If you prefer embedding into Squarespace instead of GitHub Pages, you can place the HTML/CSS/JS into a Code Block on a page — however full page hosting via GitHub Pages is simpler.

I can now:
- Push the `web/` folder into the repository and create a small `web/examples/` folder with placeholder images/text so visitors who clone the repo can test immediately.
- Provide exact DNS screenshot instructions for Squarespace domain settings.

Which should I do now?


