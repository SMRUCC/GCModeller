﻿# Standards for Command Line Interfaces

> https://www.gnu.org/prep/standards/html_node/Command_002dLine-Interfaces.html

It is a good idea to follow the POSIX guidelines for the command-line options of a program. The 
easiest way to do this is to use getopt to parse them. Note that the GNU version of getopt will 
normally permit options anywhere among the arguments unless the special argument ``--`` is used. 
This is not what POSIX specifies; it is a GNU extension.

Please define long-named options that are equivalent to the single-letter Unix-style options. We 
hope to make GNU more user friendly this way. This is easy to do with the GNU function 
``getopt_long``.

One of the advantages of long-named options is that they can be consistent from program to program.
For example, users should be able to expect the “verbose” option of any GNU program which has 
one, to be spelled precisely ``--verbose``. To achieve this uniformity, look at the table of 
common long-option names when you choose the option names for your program (see Option Table).

It is usually a good idea for file names given as ordinary arguments to be input files only; any 
output files would be specified using options (preferably ``-o`` or ``--output``). Even if you 
allow an output file name as an ordinary argument for compatibility, try to provide an option as 
another way to specify it. This will lead to more consistency among GNU utilities, and fewer 
idiosyncrasies for users to remember.

All programs should support two standard options: ``--version`` and ``--help``. CGI programs should 
accept these as command-line options, and also if given as the PATH_INFO; for instance, visiting 
‘http://example.org/p.cgi/--help’ in a browser should output the same information as invoking 
``p.cgi --help`` from the command line.

## ``--version``

The standard ``--version`` option should direct the program to print information about its name, version, 
origin and legal status, all on standard output, and then exit successfully. Other options and 
arguments should be ignored once this is seen, and the program should not perform its normal function.

The first line is meant to be easy for a program to parse; the version number proper starts after the 
last space. In addition, it contains the canonical name for this program, in this format:

```
GNU Emacs 19.30
```

The program’s name should be a constant string; don’t compute it from ``argv[0]``. The idea is to state 
the standard or canonical name for the program, not its file name. There are other ways to find out 
the precise file name where a command is found in PATH.

If the program is a subsidiary part of a larger package, mention the package name in parentheses, like 
this:

```
emacsserver (GNU Emacs) 19.30
```

If the package has a version number which is different from this program’s version number, you can 
mention the package version number just before the close-parenthesis.

If you need to mention the version numbers of libraries which are distributed separately from the package 
which contains this program, you can do so by printing an additional line of version info for each 
library you want to mention. Use the same format for these lines as for the first line.

Please do not mention all of the libraries that the program uses “just for completeness”—that would 
produce a lot of unhelpful clutter. Please mention library version numbers only if you find in practice 
that they are very important to you in debugging.

The following line, after the version number line or lines, should be a copyright notice. If more than 
one copyright notice is called for, put each on a separate line.

Next should follow a line stating the license, preferably using one of abbreviations below, and a brief 
statement that the program is free software, and that users are free to copy and change it. Also mention 
that there is no warranty, to the extent permitted by law. See recommended wording below.

It is ok to finish the output with a list of the major authors of the program, as a way of giving credit.

Here’s an example of output that follows these rules:

```
GNU hello 2.3
Copyright (C) 2007 Free Software Foundation, Inc.
License GPLv3+: GNU GPL version 3 or later <https://gnu.org/licenses/gpl.html>
This is free software: you are free to change and redistribute it.
There is NO WARRANTY, to the extent permitted by law.
```

You should adapt this to your program, of course, filling in the proper year, copyright holder, name of 
program, and the references to distribution terms, and changing the rest of the wording as necessary.

This copyright notice only needs to mention the most recent year in which changes were made—there’s no 
need to list the years for previous versions’ changes. You don’t have to mention the name of the program 
in these notices, if that is inconvenient, since it appeared in the first line. (The rules are different 
for copyright notices in source files; see Copyright Notices in Information for GNU Maintainers.)

Translations of the above lines must preserve the validity of the copyright notices (see 
Internationalization). If the translation’s character set supports it, the ‘(C)’ should be replaced 
with the copyright symbol, as follows:

```
©
```

Write the word “Copyright” exactly like that, in English. Do not translate it into another language. 
International treaties recognize the English word “Copyright”; translations into other languages do 
not have legal significance.

Finally, here is the table of our suggested license abbreviations. Any abbreviation can be followed by 
``vversion[+]``, meaning that particular version, or later versions with the ‘+’, as shown above. In 
the case of a GNU license, always indicate the permitted versions in this way.

In the case of exceptions for extra permissions with the GPL, we use ‘/’ for a separator; the version 
number can follow the license abbreviation as usual, as in the examples below.

GPL
GNU General Public License, https://www.gnu.org/licenses/gpl.html.

LGPL
GNU Lesser General Public License, https://www.gnu.org/licenses/lgpl.html.

GPL/Ada
GNU GPL with the exception for Ada.

Apache
The Apache Software Foundation license, https://directory.fsf.org/wiki/License:Apache2.0.

Artistic
The Artistic license used for Perl, https://directory.fsf.org/wiki/License:ArtisticLicense2.0.

Expat
The Expat license, https://directory.fsf.org/wiki/License:Expat.

MPL
The Mozilla Public License, https://directory.fsf.org/wiki/License:MPLv2.0.

OBSD
The original (4-clause) BSD license, incompatible with the GNU GPL,
https://directory.fsf.org/wiki/License:BSD_4Clause.

PHP
The license used for PHP, https://directory.fsf.org/wiki/License:PHPv3.01.

public domain
The non-license that is being in the public domain,
https://www.gnu.org/licenses/license-list.html#PublicDomain.

Python
The license for Python, https://directory.fsf.org/wiki/License:Python2.0.1.

RBSD
The revised (3-clause) BSD, compatible with the GNU GPL,
https://directory.fsf.org/wiki/License:BSD_3Clause.

X11
The simple non-copyleft license used for most versions of the X Window System, 
https://directory.fsf.org/wiki/License:X11.

Zlib
The license for Zlib, https://directory.fsf.org/wiki/License:Zlib.

More information about these licenses and many more are on the GNU licensing web pages, 
https://www.gnu.org/licenses/license-list.html.

## --help

The standard --help option should output brief documentation for how to invoke the program, on standard 
output, then exit successfully. Other options and arguments should be ignored once this is seen, and 
the program should not perform its normal function.

Near the end of the ``--help`` option’s output, please place lines giving the email address for bug 
reports, the package’s home page (normally ``https://www.gnu.org/software/pkg``, and the general page 
for help using GNU programs. The format should be like this:

```
Report bugs to: mailing-address
pkg home page: <https://www.gnu.org/software/pkg/>
General help using GNU software: <https://www.gnu.org/gethelp/>
```

It is ok to mention other appropriate mailing lists and web pages.

## Table of Long Options

Here is a table of long options used by GNU programs. It is surely incomplete, but we aim to list all the 
options that a new program might want to be compatible with. If you use names not already in the table, 
please send bug-standards@gnu.org a list of them, with their meanings, so we can update the table.

‘after-date’
‘-N’ in tar.

‘all’
‘-a’ in du, ls, nm, stty, uname, and unexpand.

‘all-text’
‘-a’ in diff.

‘almost-all’
‘-A’ in ls.

‘append’
‘-a’ in etags, tee, time; ‘-r’ in tar.

‘archive’
‘-a’ in cp.

‘archive-name’
‘-n’ in shar.

‘arglength’
‘-l’ in m4.

‘ascii’
‘-a’ in diff.

‘assign’
‘-v’ in gawk.

‘assume-new’
‘-W’ in make.

‘assume-old’
‘-o’ in make.

‘auto-check’
‘-a’ in recode.

‘auto-pager’
‘-a’ in wdiff.

‘auto-reference’
‘-A’ in ptx.

‘avoid-wraps’
‘-n’ in wdiff.

‘background’
For server programs, run in the background.

‘backward-search’
‘-B’ in ctags.

‘basename’
‘-f’ in shar.

‘batch’
Used in GDB.

‘baud’
Used in GDB.

‘before’
‘-b’ in tac.

‘binary’
‘-b’ in cpio and diff.

‘bits-per-code’
‘-b’ in shar.

‘block-size’
Used in cpio and tar.

‘blocks’
‘-b’ in head and tail.

‘break-file’
‘-b’ in ptx.

‘brief’
Used in various programs to make output shorter.

‘bytes’
‘-c’ in head, split, and tail.

‘c++’
‘-C’ in etags.

‘catenate’
‘-A’ in tar.

‘cd’
Used in various programs to specify the directory to use.

‘changes’
‘-c’ in chgrp and chown.

‘classify’
‘-F’ in ls.

‘colons’
‘-c’ in recode.

‘command’
‘-c’ in su; ‘-x’ in GDB.

‘compare’
‘-d’ in tar.

‘compat’
Used in gawk.

‘compress’
‘-Z’ in tar and shar.

‘concatenate’
‘-A’ in tar.

‘confirmation’
‘-w’ in tar.

‘context’
Used in diff.

‘copyleft’
‘-W copyleft’ in gawk.

‘copyright’
‘-C’ in ptx, recode, and wdiff; ‘-W copyright’ in gawk.

‘core’
Used in GDB.

‘count’
‘-q’ in who.

‘count-links’
‘-l’ in du.

‘create’
Used in tar and cpio.

‘cut-mark’
‘-c’ in shar.

‘cxref’
‘-x’ in ctags.

‘date’
‘-d’ in touch.

‘debug’
‘-d’ in make and m4; ‘-t’ in Bison.

‘define’
‘-D’ in m4.

‘defines’
‘-d’ in Bison and ctags.

‘delete’
‘-D’ in tar.

‘dereference’
‘-L’ in chgrp, chown, cpio, du, ls, and tar.

‘dereference-args’
‘-D’ in du.

‘device’
Specify an I/O device (special file name).

‘diacritics’
‘-d’ in recode.

‘dictionary-order’
‘-d’ in look.

‘diff’
‘-d’ in tar.

‘digits’
‘-n’ in csplit.

‘directory’
Specify the directory to use, in various programs. In ls, it means to show directories themselves rather 
than their contents. In rm and ln, it means to not treat links to directories specially.

‘discard-all’
‘-x’ in strip.

‘discard-locals’
‘-X’ in strip.

‘dry-run’
‘-n’ in make.

‘ed’
‘-e’ in diff.

‘elide-empty-files’
‘-z’ in csplit.

‘end-delete’
‘-x’ in wdiff.

‘end-insert’
‘-z’ in wdiff.

‘entire-new-file’
‘-N’ in diff.

‘environment-overrides’
‘-e’ in make.

‘eof’
‘-e’ in xargs.

‘epoch’
Used in GDB.

‘error-limit’
Used in makeinfo.

‘error-output’
‘-o’ in m4.

‘escape’
‘-b’ in ls.

‘exclude-from’
‘-X’ in tar.

‘exec’
Used in GDB.

‘exit’
‘-x’ in xargs.

‘exit-0’
‘-e’ in unshar.

‘expand-tabs’
‘-t’ in diff.

‘expression’
‘-e’ in sed.

‘extern-only’
‘-g’ in nm.

‘extract’
‘-i’ in cpio; ‘-x’ in tar.

‘faces’
‘-f’ in finger.

‘fast’
‘-f’ in su.

‘fatal-warnings’
‘-E’ in m4.

‘file’
‘-f’ in gawk, info, make, mt, sed, and tar.

‘field-separator’
‘-F’ in gawk.

‘file-prefix’
‘-b’ in Bison.

‘file-type’
‘-F’ in ls.

‘files-from’
‘-T’ in tar.

‘fill-column’
Used in makeinfo.

‘flag-truncation’
‘-F’ in ptx.

‘fixed-output-files’
‘-y’ in Bison.

‘follow’
‘-f’ in tail.

‘footnote-style’
Used in makeinfo.

‘force’
‘-f’ in cp, ln, mv, and rm.

‘force-prefix’
‘-F’ in shar.

‘foreground’
For server programs, run in the foreground; in other words, don’t do anything special to run the server 
in the background.

‘format’
Used in ls, time, and ptx.

‘freeze-state’
‘-F’ in m4.

‘fullname’
Used in GDB.

‘gap-size’
‘-g’ in ptx.

‘get’
‘-x’ in tar.

‘graphic’
‘-i’ in ul.

‘graphics’
‘-g’ in recode.

‘group’
‘-g’ in install.

‘gzip’
‘-z’ in tar and shar.

‘hashsize’
‘-H’ in m4.

‘header’
‘-h’ in objdump and recode

‘heading’
‘-H’ in who.

‘help’
Used to ask for brief usage information.

‘here-delimiter’
‘-d’ in shar.

‘hide-control-chars’
‘-q’ in ls.

‘html’
In makeinfo, output HTML.

‘idle’
‘-u’ in who.

‘ifdef’
‘-D’ in diff.

‘ignore’
‘-I’ in ls; ‘-x’ in recode.

‘ignore-all-space’
‘-w’ in diff.

‘ignore-backups’
‘-B’ in ls.

‘ignore-blank-lines’
‘-B’ in diff.

‘ignore-case’
‘-f’ in look and ptx; ‘-i’ in diff and wdiff.

‘ignore-errors’
‘-i’ in make.

‘ignore-file’
‘-i’ in ptx.

‘ignore-indentation’
‘-I’ in etags.

‘ignore-init-file’
‘-f’ in Oleo.

‘ignore-interrupts’
‘-i’ in tee.

‘ignore-matching-lines’
‘-I’ in diff.

‘ignore-space-change’
‘-b’ in diff.

‘ignore-zeros’
‘-i’ in tar.

‘include’
‘-i’ in etags; ‘-I’ in m4.

‘include-dir’
‘-I’ in make.

‘incremental’
‘-G’ in tar.

‘info’
‘-i’, ‘-l’, and ‘-m’ in Finger.

‘init-file’
In some programs, specify the name of the file to read as the user’s init file.

‘initial’
‘-i’ in expand.

‘initial-tab’
‘-T’ in diff.

‘inode’
‘-i’ in ls.

‘interactive’
‘-i’ in cp, ln, mv, rm; ‘-e’ in m4; ‘-p’ in xargs; ‘-w’ in tar.

‘intermix-type’
‘-p’ in shar.

‘iso-8601’
Used in date

‘jobs’
‘-j’ in make.

‘just-print’
‘-n’ in make.

‘keep-going’
‘-k’ in make.

‘keep-files’
‘-k’ in csplit.

‘kilobytes’
‘-k’ in du and ls.

‘language’
‘-l’ in etags.

‘less-mode’
‘-l’ in wdiff.

‘level-for-gzip’
‘-g’ in shar.

‘line-bytes’
‘-C’ in split.

‘lines’
Used in split, head, and tail.

‘link’
‘-l’ in cpio.

‘lint’
‘lint-old’
Used in gawk.

‘list’
‘-t’ in cpio; ‘-l’ in recode.

‘list’
‘-t’ in tar.

‘literal’
‘-N’ in ls.

‘load-average’
‘-l’ in make.

‘login’
Used in su.

‘machine’
Used in uname.

‘macro-name’
‘-M’ in ptx.

‘mail’
‘-m’ in hello and uname.

‘make-directories’
‘-d’ in cpio.

‘makefile’
‘-f’ in make.

‘mapped’
Used in GDB.

‘max-args’
‘-n’ in xargs.

‘max-chars’
‘-n’ in xargs.

‘max-lines’
‘-l’ in xargs.

‘max-load’
‘-l’ in make.

‘max-procs’
‘-P’ in xargs.

‘mesg’
‘-T’ in who.

‘message’
‘-T’ in who.

‘minimal’
‘-d’ in diff.

‘mixed-uuencode’
‘-M’ in shar.

‘mode’
‘-m’ in install, mkdir, and mkfifo.

‘modification-time’
‘-m’ in tar.

‘multi-volume’
‘-M’ in tar.

‘name-prefix’
‘-a’ in Bison.

‘nesting-limit’
‘-L’ in m4.

‘net-headers’
‘-a’ in shar.

‘new-file’
‘-W’ in make.

‘no-builtin-rules’
‘-r’ in make.

‘no-character-count’
‘-w’ in shar.

‘no-check-existing’
‘-x’ in shar.

‘no-common’
‘-3’ in wdiff.

‘no-create’
‘-c’ in touch.

‘no-defines’
‘-D’ in etags.

‘no-deleted’
‘-1’ in wdiff.

‘no-dereference’
‘-d’ in cp.

‘no-inserted’
‘-2’ in wdiff.

‘no-keep-going’
‘-S’ in make.

‘no-lines’
‘-l’ in Bison.

‘no-piping’
‘-P’ in shar.

‘no-prof’
‘-e’ in gprof.

‘no-regex’
‘-R’ in etags.

‘no-sort’
‘-p’ in nm.

‘no-splash’
Don’t print a startup splash screen.

‘no-split’
Used in makeinfo.

‘no-static’
‘-a’ in gprof.

‘no-time’
‘-E’ in gprof.

‘no-timestamp’
‘-m’ in shar.

‘no-validate’
Used in makeinfo.

‘no-wait’
Used in emacsclient.

‘no-warn’
Used in various programs to inhibit warnings.

‘node’
‘-n’ in info.

‘nodename’
‘-n’ in uname.

‘nonmatching’
‘-f’ in cpio.

‘nstuff’
‘-n’ in objdump.

‘null’
‘-0’ in xargs.

‘number’
‘-n’ in cat.

‘number-nonblank’
‘-b’ in cat.

‘numeric-sort’
‘-n’ in nm.

‘numeric-uid-gid’
‘-n’ in cpio and ls.

‘nx’
Used in GDB.

‘old-archive’
‘-o’ in tar.

‘old-file’
‘-o’ in make.

‘one-file-system’
‘-l’ in tar, cp, and du.

‘only-file’
‘-o’ in ptx.

‘only-prof’
‘-f’ in gprof.

‘only-time’
‘-F’ in gprof.

‘options’
‘-o’ in getopt, fdlist, fdmount, fdmountd, and fdumount.

‘output’
In various programs, specify the output file name.

‘output-prefix’
‘-o’ in shar.

‘override’
‘-o’ in rm.

‘overwrite’
‘-c’ in unshar.

‘owner’
‘-o’ in install.

‘paginate’
‘-l’ in diff.

‘paragraph-indent’
Used in makeinfo.

‘parents’
‘-p’ in mkdir and rmdir.

‘pass-all’
‘-p’ in ul.

‘pass-through’
‘-p’ in cpio.

‘port’
‘-P’ in finger.

‘portability’
‘-c’ in cpio and tar.

‘posix’
Used in gawk.

‘prefix-builtins’
‘-P’ in m4.

‘prefix’
‘-f’ in csplit.

‘preserve’
Used in tar and cp.

‘preserve-environment’
‘-p’ in su.

‘preserve-modification-time’
‘-m’ in cpio.

‘preserve-order’
‘-s’ in tar.

‘preserve-permissions’
‘-p’ in tar.

‘print’
‘-l’ in diff.

‘print-chars’
‘-L’ in cmp.

‘print-data-base’
‘-p’ in make.

‘print-directory’
‘-w’ in make.

‘print-file-name’
‘-o’ in nm.

‘print-symdefs’
‘-s’ in nm.

‘printer’
‘-p’ in wdiff.

‘prompt’
‘-p’ in ed.

‘proxy’
Specify an HTTP proxy.

‘query-user’
‘-X’ in shar.

‘question’
‘-q’ in make.

‘quiet’
Used in many programs to inhibit the usual output. Every program accepting ‘--quiet’ should accept 
‘--silent’ as a synonym.

‘quiet-unshar’
‘-Q’ in shar

‘quote-name’
‘-Q’ in ls.

‘rcs’
‘-n’ in diff.

‘re-interval’
Used in gawk.

‘read-full-blocks’
‘-B’ in tar.

‘readnow’
Used in GDB.

‘recon’
‘-n’ in make.

‘record-number’
‘-R’ in tar.

‘recursive’
Used in chgrp, chown, cp, ls, diff, and rm.

‘reference’
‘-r’ in touch.

‘references’
‘-r’ in ptx.

‘regex’
‘-r’ in tac and etags.

‘release’
‘-r’ in uname.

‘reload-state’
‘-R’ in m4.

‘relocation’
‘-r’ in objdump.

‘rename’
‘-r’ in cpio.

‘replace’
‘-i’ in xargs.

‘report-identical-files’
‘-s’ in diff.

‘reset-access-time’
‘-a’ in cpio.

‘reverse’
‘-r’ in ls and nm.

‘reversed-ed’
‘-f’ in diff.

‘right-side-defs’
‘-R’ in ptx.

‘same-order’
‘-s’ in tar.

‘same-permissions’
‘-p’ in tar.

‘save’
‘-g’ in stty.

‘se’
Used in GDB.

‘sentence-regexp’
‘-S’ in ptx.

‘separate-dirs’
‘-S’ in du.

‘separator’
‘-s’ in tac.

‘sequence’
Used by recode to chose files or pipes for sequencing passes.

‘shell’
‘-s’ in su.

‘show-all’
‘-A’ in cat.

‘show-c-function’
‘-p’ in diff.

‘show-ends’
‘-E’ in cat.

‘show-function-line’
‘-F’ in diff.

‘show-tabs’
‘-T’ in cat.

‘silent’
Used in many programs to inhibit the usual output. Every program accepting ‘--silent’ should accept 
‘--quiet’ as a synonym.

‘size’
‘-s’ in ls.

‘socket’
Specify a file descriptor for a network server to use for its socket, instead of opening and binding a 
new socket. This provides a way to run, in a non-privileged process, a server that normally needs a 
reserved port number.

‘sort’
Used in ls.

‘source’
‘-W source’ in gawk.

‘sparse’
‘-S’ in tar.

‘speed-large-files’
‘-H’ in diff.

‘split-at’
‘-E’ in unshar.

‘split-size-limit’
‘-L’ in shar.

‘squeeze-blank’
‘-s’ in cat.

‘start-delete’
‘-w’ in wdiff.

‘start-insert’
‘-y’ in wdiff.

‘starting-file’
Used in tar and diff to specify which file within a directory to start processing with.

‘statistics’
‘-s’ in wdiff.

‘stdin-file-list’
‘-S’ in shar.

‘stop’
‘-S’ in make.

‘strict’
‘-s’ in recode.

‘strip’
‘-s’ in install.

‘strip-all’
‘-s’ in strip.

‘strip-debug’
‘-S’ in strip.

‘submitter’
‘-s’ in shar.

‘suffix’
‘-S’ in cp, ln, mv.

‘suffix-format’
‘-b’ in csplit.

‘sum’
‘-s’ in gprof.

‘summarize’
‘-s’ in du.

‘symbolic’
‘-s’ in ln.

‘symbols’
Used in GDB and objdump.

‘synclines’
‘-s’ in m4.

‘sysname’
‘-s’ in uname.

‘tabs’
‘-t’ in expand and unexpand.

‘tabsize’
‘-T’ in ls.

‘terminal’
‘-T’ in tput and ul. ‘-t’ in wdiff.

‘text’
‘-a’ in diff.

‘text-files’
‘-T’ in shar.

‘time’
Used in ls and touch.

‘timeout’
Specify how long to wait before giving up on some operation.

‘to-stdout’
‘-O’ in tar.

‘total’
‘-c’ in du.

‘touch’
‘-t’ in make, ranlib, and recode.

‘trace’
‘-t’ in m4.

‘traditional’
‘-t’ in hello; ‘-W traditional’ in gawk; ‘-G’ in ed, m4, and ptx.

‘tty’
Used in GDB.

‘typedefs’
‘-t’ in ctags.

‘typedefs-and-c++’
‘-T’ in ctags.

‘typeset-mode’
‘-t’ in ptx.

‘uncompress’
‘-z’ in tar.

‘unconditional’
‘-u’ in cpio.

‘undefine’
‘-U’ in m4.

‘undefined-only’
‘-u’ in nm.

‘update’
‘-u’ in cp, ctags, mv, tar.

‘usage’
Used in gawk; same as ‘--help’.

‘uuencode’
‘-B’ in shar.

‘vanilla-operation’
‘-V’ in shar.

‘verbose’
Print more information about progress. Many programs support this.

‘verify’
‘-W’ in tar.

‘version’
Print the version number.

‘version-control’
‘-V’ in cp, ln, mv.

‘vgrind’
‘-v’ in ctags.

‘volume’
‘-V’ in tar.

‘what-if’
‘-W’ in make.

‘whole-size-limit’
‘-l’ in shar.

‘width’
‘-w’ in ls and ptx.

‘word-regexp’
‘-W’ in ptx.

‘writable’
‘-T’ in who.

‘zeros’
‘-z’ in gprof.