---
title: options
---

# options
_namespace: [RDotNET.Extensions.VisualBasic.base](N-RDotNET.Extensions.VisualBasic.base.html)_

Allow the user to set and examine a variety of global options which affect the way in which R computes and displays its results.
 (由于options函数会设置环境变量，由于设置的变量很少，但是大部分参数为逻辑值，所以在这里使用RExpression类型来防止默认为True的参数被误设置为False)

> 
>  Invoking options() with no arguments returns a list with the current values of the options. Note that not all options listed below are set initially. To access the value of a single option, one should use, e.g., getOption("width") rather than options("width") which is a list of length one.
>  For options(), a list of all set options sorted by name. For options(name), a list of length one containing the set value, or NULL if it is unset. For uses setting one or more options, a list with the previous values of the options changed (returned invisibly).
>  



### Properties

#### addSmooth
typically logical, defaulting To True. Could also be Set To an Integer For specifying how many (simulated) smooths should be added. This Is currently only used by plot.lm.
#### BioC_mirror
The URL Of a Bioconductor mirror For use by setRepositories, e.g. the Default "http://bioconductor.org" Or the European mirror "http://bioconductor.statistik.tu-dortmund.de". 
 Can be Set by chooseBioCmirror.
#### browser
The HTML browser To be used by browseURL. This sets the Default browser On UNIX Or a non-Default browser On Windows. 
 Alternatively, an R Function that Is called With a URL As its argument. See browseURL For further details.
#### browserNLdisabled
logical: whether newline Is disabled As a synonym For "n" In the browser.
#### CBoundsCheck
logical, controlling whether .C And .Fortran make copies to check for array over-runs on the atomic vector arguments.
 Initially set from value of the environment variable R_C_BOUNDS_CHECK (set to yes to enable).
#### ccaddress
Default Cc: address used by create.post (And hencebug.report And help.request). Can be False Or "".
#### checkBounds
logical, defaulting to FALSE. If true, a warning Is produced whenever a vector (atomic Or list) Is extended, by something Like x <- 13; x[5] <- 6.
#### checkPackageLicense
logical, Not set by default. If true, library asks a user to accept any non-standard license at first use.
#### citationBibtexMax
Default 1; the maximal number of bibentries (bibentry) in a citation for which the bibtex version Is printed in addition to the text one.
#### continue
a non-empty String setting the prompt used For lines which Continue over one line.
#### contrasts
the Default contrasts used In model fitting such As With aov Or lm. 
 A character vector Of length two, the first giving the Function To be used With unordered factors And the second the Function To be used With ordered factors. 
 By Default the elements are named c("unordered", "ordered"), but the names are unused.
#### deCellwidth
Integer: the cell widths (number Of characters) To be used In the data editor dataentry. If this Is unset (the Default), 0, negative Or NA, variable cell widths are used.
#### defaultPackages
the packages that are attached by Default When R starts up. 
 Initially Set from value Of the environment variable R_DEFAULT_PACKAGES, Or If that Is unset To c("datasets", "utils", "grDevices", "graphics", "stats", "methods"). (Set R_DEFAULT_PACKAGES To NULL Or a comma-separated list Of package names.) It will Not work To Set this In a '.Rprofile’ file, as its value is consulted before that file is read.
#### demoAsk
Default For the ask argument Of demo.
#### deparseCutoff
Integer value controlling the printing of language constructs which are deparsed. Default 60.
#### deparseMaxLines
controls the number Of lines used When deparsing In traceback, browser, And upon entry To a Function whose debugging flag Is Set. 
 Initially unset, And only used If Set To a positive Integer.
#### device
a character String giving the name Of a Function, Or the Function Object itself, which When called creates a New graphics device Of the Default type For that session. 
 The value Of this Option defaults To the normal screen device (e.g., X11, windows Or quartz) For an interactive session, And pdf In batch use Or If a screen Is Not available. 
 If Set To the name Of a device, the device Is looked For first from the Global environment (that Is down the usual search path) And Then In the grDevices Namespace.
 The Default values In interactive And non-interactive sessions are configurable via environment variables R_INTERACTIVE_DEVICE And R_DEFAULT_DEVICE respectively.
 The search logic For 'the normal screen device’ is that this is windows on Windows, and quartz if available on OS X (running at the console, and compiled into the build). 
 Otherwise X11 is used if environment variable DISPLAY is set.
#### deviceAskDefault
logical. The default for devAskNewPage("ask") when a device Is opened.
#### digits
controls the number Of digits To print When printing numeric values. It Is a suggestion only. Valid values are 1...22 With Default 7. 
 See the note In print.Default about values greater than 15.
#### digitsSecs
controls the maximum number Of digits To print When formatting time values In seconds. Valid values are 0...6 With Default 0. See strftime.
#### downloadFileExtra
Extra command-line argument(s) For non-Default methods: see download.file.
#### downloadFileMethod
Method to be used for download.file. Currently download methods "internal", "wininet" (Windows only), "libcurl", "wget" And "curl" are available. If Not set, method = "auto" Is chosen: see download.file.
#### dvipscmd
character string giving a command to be used in the (deprecated) off-line printing of help pages via PostScript. Defaults to "dvips".
#### echo
logical. Only used in non-interactive mode, when it controls whether input Is echoed. Command-line option --slave sets this to FALSE, but otherwise it starts the session as TRUE.
#### editor
a non-empty String Or a Function that sets the Default text editor, e.g., For edit. Set from the environment variable EDITOR On UNIX, Or If unset VISUAL Or vi.
#### encoding
The name Of an encoding, Default "native.enc". See connections.
#### error
either a Function Or an expression governing the handling Of non-catastrophic errors such As those generated by Stop As well As by signals And internally detected errors. 
 If the Option Is a Function, a Call To that Function, With no arguments, Is generated As the expression. 
 The Default value Is NULL: see stop for the behaviour in that case. The functions dump.frames And recover provide alternatives that allow post-mortem debugging. 
 Note that these need to specified as e.g. options(error = utils:recover) in startup files such as '.Rprofile’.
#### exampleAsk
Default For the ask argument Of example.
#### expressions
sets a limit On the number Of nested expressions that will be evaluated. Valid values are 25...500000 With Default 5000. 
 If you increase it, you may also want To start R With a larger protection stack; see --max-ppsize In Memory. 
 Note too that you may cause a segfault from overflow Of the C stack, And On OSes where it Is possible you may want To increase that. 
 Once the limit Is reached an Error Is thrown. The current number under evaluation can be found by calling Cstack_info.
#### help_type
Default For an argument Of help, used also As the help type by ?.
#### helpPorts
optional integer vector for setting ports of the internal HTTP server, see startDynamicHelp.
#### helpSearchTypes
Default types Of documentation To be searched by help.search And ??.
#### helpTryAllPackages
Default For an argument Of help.
#### HTTPUserAgent
String used as the user agent in HTTP(S) requests. If NULL, requests will be made without a user agent header. The default Is R (<version> <platform> <arch> <os>) .
#### installLock
logical: should per-directory package locking be used by install.packages? 
 Most useful For binary installs On OS X And Windows, but can be used In a startup file For source installs via R CMD INSTALL. 
 For binary installs, can also be the character String "pkgloack".
#### installPackagesCheckSource
Used by install.packages (And indirectly update.packages) On platforms which support binary packages. Possible values "yes" And "no", With unset being equivalent To "yes".
#### installPackagesCompileFromSource
Used by install.packages(type = "both") (And indirectly update.packages) On platforms which support binary packages. Possible values are "never", "interactive" 
 (which means ask In interactive use And "never" In batch use) And "always". 
 The Default Is taken from environment variable R_COMPILE_AND_INSTALL_PACKAGES, With Default "interactive" If unset. However, install.packages uses "never" unless a make program Is found, consulting the environment variable MAKE.
#### internetInfo
The minimum level Of information To be printed On URL downloads etc, Using the "internal" And "libcurl" methods. Default Is 2, For failure causes. 
 Set To 1 Or 0 To Get more detailed information (For the "internal" method 0 provides more information than 1).
#### keepSource
When TRUE, the source code for functions (newly defined Or loaded) Is stored internally allowing comments to be kept in the right places. 
 Retrieve the source by printing Or using deparse(fn, control = "useSource").
 The Default Is interactive(), i.e., True For interactive use.
#### keepSourcePkgs
As for keep.source, used only when packages are installed. Defaults to FALSE unless the environment variable R_KEEP_PKG_SOURCE Is set to yes.
#### locatorBell
logical. Should selection in locator And identify be confirmed by a bell? Default TRUE. Honoured at least on X11 And windows devices.
#### mailer
Default emailing method used by create.post And hence bug.report And help.request.
#### maxContourSegments
positive integer, defaulting to 25000 if Not set. A limit on the number of segments in a single contour line in contour Or contourLines.
#### maxPrint
Integer, defaulting To 99999. print Or show methods can make use Of this Option, to limit the amount of information that Is printed, to something in the order of (And typically slightly less than) max.print entries.
#### mcCores
a integer giving the maximum allowed number of additional R processes allowed to be run in parallel to the current R process. 
 Defaults to the setting of the environment variable MC_CORES if set. Most applications which use this assume a limit of 2 if it Is unset.
#### menuGraphics
Logical: should graphical menus be used If available?. Defaults To True. Currently applies To Select.list, chooseCRANmirror, setRepositories And To Select from multiple (text) help files In help.
#### NAaction
the name Of a Function For treating missing values (NA's) for certain situations.
#### nwarnings
the limit For the number Of warnings kept When warn = 0, Default 50. This will discard messages If called whilst they are being collected.
#### OutDec
character string containing a single character. The preferred character to be used as the decimal point in output conversions, that Is in printing, plotting, format And as.character but Not when deparsing nor by sprintf nor formatC (which are sometimes used prior to printing.)
 Only single-byte characters were supported prior to R 3.2.0. In R 3.2.1 And earlier, multi- (Or zero-) character OutDec were accepted, but always worked only partially.
#### pager
the command used For displaying text files by file.show. Defaults To "internal", which uses a pager similar To the GUI console. Another possibility Is "console" To use the console itself. 
 Can be a character String Or an R Function, In which Case it needs To accept the arguments (files, header, title, delete.file) corresponding To the first four arguments Of file.show.
#### papersize
the Default paper format used by postscript; Set by environment variable R_PAPERSIZE When R Is started: If that Is unset Or invalid it defaults To "a4", Or "letter" In US And Canadian locales.
#### pdfviewer
Default PDF viewer. The Default Is Set from the environment variable R_PDFVIEWER, which defaults To the full path To open.exe, a utility supplied With R.
#### pkgType
The Default type Of packages To be downloaded And installed – see install.packages. Possible values are "win.binary", "source" And "both" (the Default). 
 Some OS X builds use "mac.binary" And "mac.binary.mavericks". Value "binary" Is a synonym For the native binary type (If there Is one); "both" Is used by install.packages To choose between source And binary installs.
#### printcmd
the command used by postscript For printing; Set by environment variable R_PRINTCMD When R Is started. 
 This should be a command that expects either input To be piped To 'stdin’ or to be given a single filename argument. Usually set to "lpr" on a Unix-alike.
#### prompt
a non-empty String To be used For R's prompt; should usually end in a blank (" ").
#### repos
URLs of the repositories for use by update.packages. Defaults to c(CRAN="@CRAN@"), a value that causes some utilities to prompt for a CRAN mirror. 
 To avoid this do set the CRAN mirror, by something Like local({r <- getOption("repos"); r["CRAN"] <- "http://my.local.cran"; options(repos = r)}).
 Note that you can add more repositories (Bioconductor And Omegahat, R-Forge, Rforge.net ...) Using setRepositories.
#### scipen
Integer.A penalty to be applied when deciding to print numeric values in fixed Or exponential notation. 
 Positive values bias towards fixed And negative towards scientific notation: fixed notation will be preferred unless it Is more than scipen digits wider.
#### showCoefPvalues
logical, affecting whether P values are printed in summary tables of coefficients. See printCoefmat.
#### showErrorLocations
Should source locations Of errors be printed? If Set To True Or "top", the source location that Is highest On the stack (the most recent Call) will be printed. 
 "bottom" will print the location Of the earliest Call found On the stack.
 
 Integer values can select other entries. The value 0 corresponds to "top" And positive values count down the stack from there. 
 The value -1 corresponds to "bottom" And negative values count up from there.
#### showErrorMessages
a logical. Should Error messages be printed? Intended For use With Try Or a user-installed Error handler.
#### showNCalls
Integer.Controls how long the sequence of calls must be (in bytes) before ellipses are used. Defaults to 40 And should be at least 30 And no more than 500.
#### showNlsConvergence
logical, should nls convergence messages be printed for successful fits?
#### showSignifStars
logical, should stars be printed on summary tables of coefficients? See printCoefmat.
#### showWarnCalls
a logical. Should warning And Error messages show a summary Of the Call stack? By Default Error calls are shown In non-interactive sessions.
#### stringsAsFactors
The Default setting For arguments Of data.frame And read.table.
#### texi2dvi
used by functions texi2dvi And texi2pdf In package tools.
#### timeout
Integer.The timeout For some Internet operations, in seconds. Default 60 seconds. See download.file And connections.
#### topLevelEnvironment
see topenv And sys.source.
#### tsEps
the relative tolerance For certain time series (ts) computations. Default 1E-05.
#### tsSCompat
logical. Used to select S compatibility for plotting time-series spectra. See the description of argument log in plot.spec.
#### unzip
a character String used by unzip: the path Of the external program unzip Or "internal". Defaults To "internal" When the internal unzip code Is used.
#### urlMethod
character string: the Default method For url. Normally unset, which Is equivalent To "default", which Is "internal" except On Windows.
#### useFancyQuotes
controls the use Of directional quotes In sQuote, dQuote And In rendering text help (see Rd2txt In package tools). Can be True, False, "TeX" Or "UTF-8".
#### useHTTPS
logical. Used by chooseCRANmirror: are secure mirrors preferred? If Not Set, True Is assumed.
#### verbose
logical. Should R report extra information on progress? Set to TRUE by the command-line option --verbose.
#### warn
sets the handling Of warning messages. If warn Is negative all warnings are ignored. If warn Is zero (the Default) warnings are stored until the top–level Function returns. 
 If 10 Or fewer warnings were signalled they will be printed otherwise a message saying how many were signalled. 
 An Object called last.warning Is created And can be printed through the Function warnings. If warn Is one, warnings are printed As they occur. 
 If warn Is two Or larger all warnings are turned into errors.
#### warnFPU
logical, by default undefined. If true, a warning Is produced whenever dyn.load repairs the control word damaged by a buggy DLL.
#### warningExpression
an R code expression To be called If a warning Is generated, replacing the standard message. If non-null it Is called irrespective Of the value Of Option warn.
#### warningLength
sets the truncation limit For Error And warning messages. A non-negative Integer, With allowed values 100...8170, Default 1000.
#### warnPartialMatchArgs
logical. If true, warns if partial matching Is used in argument matching.
#### warnPartialMatchAttr
logical. If true, warns if partial matching Is used in extracting attributes via attr.
#### warnPartialMatchDollar
logical. If true, warns if partial matching Is used for extraction by $.
#### width
controls the maximum number Of columns On a line used In printing vectors, matrices And arrays, And When filling by cat.
 Columns are normally the same As characters except In East Asian languages.
 
 You may want To change this If you re-size the window that R Is running In. Valid values are 10...10000 With Default normally 80. (The limits On valid values are In file 'Print.h’ and can be changed by re-compiling R.) Some R consoles automatically change the value when they are resized.
 See the examples On Startup For one way To Set this automatically from the terminal width When R Is started.
#### windowsTimeout
(Windows-only) integer vector of length 2 representing two times in milliseconds. 
 These control the double-buffering of windows devices when that Is enabled: the first Is the delay after plotting finishes (Default 100) And the second Is the update interval during continuous plotting (Default 500). The values at the time the device Is opened are used.
