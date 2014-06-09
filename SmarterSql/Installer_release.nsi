; The name of the installer
Name "SmarterSql"

!define WNDCLASS "wndclass_desked_gsk"
!define WNDTITLE "Microsoft SQL Server Management Studio"

!define SourcePath SmarterSql\bin\release

!define SF_SELECTED   1
!define SF_SECGRP     2
!define SF_SECGRPEND  4
!define SF_BOLD       8
!define SF_RO         16
!define SF_EXPAND     32
!define SF_PSELECTED  64

; The file to write
OutFile "Sassner_SmarterSql_setup.exe"

; The default installation directory
InstallDir $PROGRAMFILES\Sassner\SmarterSql

; Registry key to check for directory (so if you install again, it will overwrite the old one automatically)
InstallDirRegKey HKLM "Software\NSIS_SmarterSql" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

XPStyle on
InstallColors /windows
InstProgressFlags smooth

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "SmarterSql" sectionmain

	SectionIn RO

	; Set output path to the installation directory.
	SetOutPath $INSTDIR
	
	; Put file there
	File "${SourcePath}\Sassner.SmarterSql.pdb"
	File "${SourcePath}\Sassner.SmarterSql.dll"
;	File "${SourcePath}\Changelog.txt"
	File "${SourcePath}\Sassner.Sql.GlacialList.dll"
;	File "${SourcePath}\Sassner.Sql.GlacialList.pdb"
	File "${SourcePath}\..\..\..\Licensavtal.txt"
	File "${SourcePath}\..\..\..\Manual_SmarterSql.doc"

	;To Register a DLL
	nsExec::Exec '"$WINDIR\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe" /codebase "$INSTDIR\Sassner.SmarterSql.dll"'

	; Write the installation path into the registry
	WriteRegStr HKLM SOFTWARE\NSIS_SmarterSql "Install_Dir" "$INSTDIR"

	; Write the uninstall keys for Windows
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SmarterSql" "DisplayName" "SmarterSql"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SmarterSql" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SmarterSql" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SmarterSql" "NoRepair" 1
	WriteUninstaller "uninstall.exe"

SectionEnd

;--------------------------------

SectionGroup /e "Sql Management Studio version"

Section /o "Sql Management Studio 2005" sectionssms2005

	File "${SourcePath}\Sassner.SmarterSql.Addin2005.dll"
	File "${SourcePath}\Sassner.SmarterSql.Addin2005.pdb"

	WriteRegStr HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\90\Tools\Shell\Addins\Sassner.SmarterSql.Connect" "Description" "Smarter Sql"
	WriteRegStr HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\90\Tools\Shell\Addins\Sassner.SmarterSql.Connect" "FriendlyName" "Smarter Sql addin, SQL Server Management Studio Extension"
	WriteRegDWORD HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\90\Tools\Shell\Addins\Sassner.SmarterSql.Connect" "LoadBehavior" 1

SectionEnd

Section /o "Sql Management Studio 2008" sectionssms2008

	File "${SourcePath}\Sassner.SmarterSql.Addin2008.dll"
	File "${SourcePath}\Sassner.SmarterSql.Addin2008.pdb"

	WriteRegStr HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\Shell\Addins\Sassner.SmarterSql.Connect" "Description" "Smarter Sql"
	WriteRegStr HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\Shell\Addins\Sassner.SmarterSql.Connect" "FriendlyName" "Smarter Sql addin, SQL Server Management Studio Extension"
	WriteRegDWORD HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\Shell\Addins\Sassner.SmarterSql.Connect" "LoadBehavior" 1

SectionEnd

SectionGroupEnd

;--------------------------------

Section "NET Framework 3.5" sectionframework
	;registry
	ReadRegDWORD $0 HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5' Version

	StrCpy $0 $0 3 # strip all but the first 3 chars

	StrCmp $0 '3.5' alreadyinstalled
		SetOutPath '$TEMP'
		SetOverwrite on
		File 'dotNetFx35setup.exe'
		ExecWait '$TEMP\dotNetFx35setup.exe /norestart' $0
		DetailPrint '..NET Framework 3.5 exit code = $0'
		Delete '$TEMP\dotNetFx35setup.exe'

		goto end
		
	alreadyinstalled:
		DetailPrint '..NET Framework 3.5 already installed !!'

	end:
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts" sectionmenushortcuts

	CreateDirectory "$SMPROGRAMS\SmarterSql"
	CreateShortCut "$SMPROGRAMS\SmarterSql\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
	CreateShortCut "$SMPROGRAMS\SmarterSql\Manual_SmarterSql.doc.lnk" "$INSTDIR\Manual_SmarterSql.doc" "" "$INSTDIR\Manual_SmarterSql.doc" 0
	
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
	
	; Remove registry keys
	DeleteRegKey HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\90\Tools\Shell\Addins\Sassner.SmarterSql.Connect"
	DeleteRegKey HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\Shell\Addins\Sassner.SmarterSql.Connect"

	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SmarterSql"
	DeleteRegKey HKLM SOFTWARE\NSIS_SmarterSql

	nsExec::Exec '"$WINDIR\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe" /unregister "$INSTDIR\Sassner.SmarterSql.dll"'

	; Remove files and uninstaller
	Delete $INSTDIR\Sassner.SmarterSql.Addin2005.dll
	Delete $INSTDIR\Sassner.SmarterSql.Addin2005.pdb
	Delete $INSTDIR\Sassner.SmarterSql.Addin2008.dll
	Delete $INSTDIR\Sassner.SmarterSql.Addin2008.pdb

	Delete $INSTDIR\Sassner.SmarterSql.pdb
	Delete $INSTDIR\Sassner.SmarterSql.dll
	Delete $INSTDIR\Changelog.txt
	Delete $INSTDIR\Sassner.Sql.GlacialList.dll
	Delete $INSTDIR\Sassner.Sql.GlacialList.pdb
	Delete $INSTDIR\Licensavtal.txt
	Delete $INSTDIR\Manual_SmarterSql.doc

	Delete $INSTDIR\uninstall.exe

	; Remove shortcuts, if any
	Delete "$SMPROGRAMS\SmarterSql\*.*"

	; Remove directories used
	RMDir "$SMPROGRAMS\SmarterSql"
	RMDir "$INSTDIR"

SectionEnd

;--------------------------------

;
; .oninit is called before window is shown
;
Function .onInit
	Call UnInstallOldVersion

	; Disable Intellisense in 2008
	WriteRegStr HKCU "Software\Microsoft\Microsoft SQL Server\100\Tools\Shell\SqlEditor" "EnableIntellisense" "False"

	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\90\Tools\Shell" ApplicationID
	StrCmp $0 "" SqlServer2005DoesntExists
		SectionSetFlags ${sectionssms2005} ${SF_SELECTED}
	SqlServer2005DoesntExists:

	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\Shell" ApplicationID
	StrCmp $0 "" SqlServer2008DoesntExists
		SectionSetFlags ${sectionssms2008} ${SF_SELECTED}
	SqlServer2008DoesntExists:

	ReadRegDWORD $0 HKLM 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5' Version
	StrCpy $0 $0 3 # strip all but the first 3 chars
	StrCmp $0 '3.5' alreadyinstalled
		goto end
	alreadyinstalled:
		SectionSetFlags ${sectionframework} ${SF_PSELECTED}
		SectionSetFlags ${sectionframework} ${SF_RO}
	end:

FunctionEnd

Function UnInstallOldVersion
	; force uninstall of previous version using NSIS
	; We need to wait until the uninstaller finishes before continuing, since it's possible
	; to click the next button again before the uninstaller's window appears and takes focus.
	; This is tricky: we can't just ExecWait the uninstaller, since it copies off the uninstaller
	; EXE and exec's that (otherwise it couldn't delete itself), so it appears to exit immediately.
	; We need to copy it off ourself, run it with the hidden parameter to tell it it's a copy,
	; and then delete the copy ourself.  There's one more trick: the hidden parameter changed
	; between NSIS 1 and 2: in 1.x it was _=C:\Foo, in 2.x it's _?=C:\Foo.  Rename the installer
	; for newer versions, so we can tell the difference: "uninst.exe" is the old 1.x uninstaller,
	; "uninstall.exe" is 2.x.
	StrCpy $R1 "$INSTDIR\uninst.exe"
	StrCpy $R2 "_="
	IfFileExists "$R1" prompt_uninstall_nsis
	StrCpy $R1 "$INSTDIR\uninstall.exe"
	StrCpy $R2 "_?="
	IfFileExists "$R1" prompt_uninstall_nsis old_nsis_not_installed

	prompt_uninstall_nsis:
	MessageBox MB_YESNO|MB_ICONINFORMATION "The previous version of SmarterSql must be uninstalled before continuing.$\nDo you wish to continue?" IDYES do_uninstall_nsis
	Abort

	do_uninstall_nsis:
	GetTempFileName $R3
	CopyFiles /SILENT $R1 $R3
	ExecWait '$R3 $R2$INSTDIR' $R4
	; Delete the copy of the uninstaller.
	Delete $R3

	; $R4 is the exit value of the uninstaller.  0 means success, anything else is
	; failure (eg. aborted).
	IntCmp $R4 0 old_nsis_not_installed ; jump if 0

	MessageBox MB_YESNO|MB_DEFBUTTON2|MB_ICONINFORMATION "Uninstallation failed.  Install anyway?" IDYES old_nsis_not_installed
	Abort

	old_nsis_not_installed:

	; The 3.0 uninstaller leaves a VDI behind; delete it.
	;Delete "$INSTDIR\StepMania.vdi"
FunctionEnd

;--------------------------------

Function un.onInit
	FindWindow $0 "${WNDCLASS}" "${WNDTITLE}"
	StrCmp $0 0 continueInstall
		MessageBox MB_ICONSTOP|MB_OK "The application you are trying to remove is running. Close it and try again."
		Abort
	continueInstall:
FunctionEnd
