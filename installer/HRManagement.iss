#define MyAppName "HRManagement"
#define MyAppPublisher "HRManagement"
#define MyAppExeName "HRManagement.WinForms.exe"
#define SourceDir "..\artifacts\publish\win-x64"

[Setup]
AppId={{9A6E7569-1F36-4658-9A51-82293E2A5F7D}
AppName={#MyAppName}
AppVersion=1.0.0
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\HRManagement
DisableProgramGroupPage=yes
OutputDir=..\artifacts\installer
OutputBaseFilename=HRManagementSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
UsePreviousAppDir=yes
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "Data\*"

[Dirs]
Name: "{app}\Data"; Permissions: users-modify
Name: "{app}\Data\Database"; Permissions: users-modify
Name: "{app}\Data\Documents"; Permissions: users-modify
Name: "{app}\Data\Photos"; Permissions: users-modify
Name: "{app}\Data\LetterTemplates"; Permissions: users-modify
Name: "{app}\Data\GeneratedLetters"; Permissions: users-modify
Name: "{app}\Data\Backups"; Permissions: users-modify
Name: "{app}\Data\Trash"; Permissions: users-modify
Name: "{app}\Data\Logs"; Permissions: users-modify
Name: "{app}\Data\Temp"; Permissions: users-modify

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\Temp"

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    MsgBox('Operational data in the Data folder is preserved. Remove it manually only after taking a backup.', mbInformation, MB_OK);
  end;
end;
