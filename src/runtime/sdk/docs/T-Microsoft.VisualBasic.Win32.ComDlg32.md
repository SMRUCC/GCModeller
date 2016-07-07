---
title: ComDlg32
---

# ComDlg32
_namespace: [Microsoft.VisualBasic.Win32](N-Microsoft.VisualBasic.Win32.html)_





### Methods

#### CommDlgExtendedError
```csharp
Microsoft.VisualBasic.Win32.ComDlg32.CommDlgExtendedError
```
CommDlgExtendedError returns the error code from the last common dialog box function. This function does not return error codes for any other API function; for that, use GetLastError instead.
_returns: 
 The function's return value is undefined if the last common
 dialog function call was successful. If an error with a common dialog function did occur, the return value is exactly one of the
 following common dialog error flags: 
 _


### Properties

#### CDERR_DIALOGFAILURE
The function could not open the dialog box.
#### CDERR_FINDRESFAILURE
The function failed to find the desired resource.
#### CDERR_GENERALCODES
The error involved a general common dialog box property.
#### CDERR_INITIALIZATION
The function failed during initialization (probably insufficient memory).
#### CDERR_LOADRESFAILURE
The function failed to load the desired resource.
#### CDERR_LOADSTRFAILURE
The function failed to load the desired string.
#### CDERR_LOCKRESFAILURE
The function failed to lock the desired resource.
#### CDERR_MEMALLOCFAILURE
The function failed to allocate sufficient memory.
#### CDERR_MEMLOCKFAILURE
The function failed to lock the desired memory.
#### CDERR_NOHINSTANCE
The function was not provided with a valid instance handle (if one was required).
#### CDERR_NOHOOK
The function was not provided with a valid hook function handle (if one was required).
#### CDERR_NOTEMPLATE
The function was not provided with a valid template (if one was required).
#### CDERR_REGISTERMSGFAIL
The function failed to successfully register a window message.
#### CDERR_STRUCTSIZE
The function was provided with an invalid structure size.
#### CFERR_CHOOSEFONTCODES
The error involved the Choose Font common dialog box.
#### CFERR_MAXLESSTHANMIN
The function was provided with a maximum font size value smaller than the provided minimum font size.
#### CFERR_NOFONTS
The function could not find any existing fonts.
#### FNERR_BUFFERTOOSMALL
The function was provided with a filename buffer which was too small.
#### FNERR_FILENAMECODES
The error involved the Open File or Save File common dialog box.
#### FNERR_INVALIDFILENAME
The function was provided with or received an invalid filename.
#### FNERR_SUBCLASSFAILURE
The function had insufficient memory to subclass the list box.
#### FRERR_BUFFERLENGTHZERO
The function was provided with an invalid buffer.
#### FRERR_FINDREPLACECODES
The error involved the Find or Replace common dialog box.
#### PDERR_CREATEICFAILURE
The function failed to create an information context.
#### PDERR_DEFAULTDIFFERENT
The function was told that the information provided described the default printer, but the default printer's actual settings were
 different.
#### PDERR_DNDMMISMATCH
The data in the two data structures describe different printers (i.e., they hold conflicting information).
#### PDERR_GETDEVMODEFAIL
The printer driver failed to initialize the DEVMODE structure.
#### PDERR_INITFAILURE
The function failed during initialization.
#### PDERR_LOADDRVFAILURE
The function failed to load the desired device driver.
#### PDERR_NODEFAULTPRN
The function could not find a default printer.
#### PDERR_NODEVICES
The function could not find any printers.
#### PDERR_PARSEFAILURE
The function failed to parse the printer-related strings in WIN.INI.
#### PDERR_PRINTERCODES
The error involved the Print common dialog box.
#### PDERR_PRINTERNOTFOUND
The function could not find information in WIN.INI about the requested printer.
#### PDERR_RETDEFFAILURE
The handles to the data structures provided were nonzero even though the function was asked to return information about
 the default printer.
#### PDERR_SETUPFAILURE
The function failed to load the desired resources.
