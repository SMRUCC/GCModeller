#Region "Microsoft.VisualBasic::166d76d12569bdbcb9fd7849e36b4b8c, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\API\base\base.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports Microsoft.VisualBasic.Linq

Namespace API

    Public Module base

        ''' <summary>
        ''' Remove Objects from a Specified Environment
        ''' 
        ''' ``remove`` and ``rm`` can be used to remove objects. These can be specified 
        ''' successively as character strings, or in the character vector list, or 
        ''' through a combination of both. 
        ''' 
        ''' All objects thus specified will be removed.
        ''' </summary>
        ''' <param name="list$">the objects To be removed, As names (unquoted) Or character strings (quoted). a character vector naming objects to be removed.</param>
        ''' <param name="pos%">where to do the removal. By default, uses the current environment. See ‘details’ for other possibilities.</param>
        ''' <param name="envir$">the environment to use. See ‘details’.</param>
        ''' <param name="[inherits]">should the enclosing frames of the environment be inspected?</param>
        Public Sub rm(list$,
                      Optional pos% = -1,
                      Optional envir$ = "as.environment(pos)",
                      Optional [inherits] As Boolean = False)
            SyncLock R
                With R
                    .call = $"rm(list = {list}, pos = {pos}, envir = {envir}, inherits = {[inherits].λ})"
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' List Objects
        ''' 
        ''' ls and objects return a vector of character strings giving the names of the 
        ''' objects in the specified environment. When invoked with no argument at the 
        ''' top level prompt, ls shows what data sets and functions a user has defined. 
        ''' When invoked with no argument inside a function, ls returns the names of 
        ''' the function's local variables: this is useful in conjunction with browser.
        ''' </summary>
        ''' <param name="name$">which environment to use in listing the available objects. Defaults to the current environment. Although called name for back compatibility, in fact this argument can specify the environment in any form; see the ‘Details’ section.</param>
        ''' <param name="pos$">an alternative argument to name for specifying the environment as a position in the search list. Mostly there for back compatibility.</param>
        ''' <param name="envir$">an alternative argument to name for specifying the environment. Mostly there for back compatibility.</param>
        ''' <param name="allnames">a logical value. If TRUE, all object names are returned. If FALSE, names which begin with a . are omitted.</param>
        ''' <param name="pattern$">an optional regular expression. Only names matching pattern are returned. glob2rx can be used to convert wildcard patterns to regular expressions.</param>
        ''' <param name="sorted">logical indicating if the resulting character should be sorted alphabetically. Note that this is part of ls() may take most of the time.</param>
        ''' <returns></returns>
        Public Function ls(Optional name$ = NULL,
                           Optional pos$ = "-1L",
                           Optional envir$ = "as.environment(pos)",
                           Optional allnames As Boolean = False,
                           Optional pattern$ = "NULL",
                           Optional sorted As Boolean = True) As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- ls({name}, pos = {pos}, envir = {envir},
   all.names = {allnames.λ}, pattern = {pattern}, sorted = {sorted.λ})"
                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' writes an external representation of R objects to the specified file. The objects can be read back from the file at a later date by using the function load or attach (or data in some cases).
        ''' </summary>
        ''' <param name="objects">the names of the objects to be saved (as symbols or character strings).</param>
        ''' <param name="file$">a (writable binary-mode) connection or the name of the file where the data will be saved (when tilde expansion is done). Must be a file name for save.image or version = 1.</param>
        ''' <param name="ascii">if TRUE, an ASCII representation of the data is written. The default value of ascii is FALSE which leads to a binary file being written. If NA and version >= 2, a different ASCII representation is used which writes double/complex numbers as binary fractions.</param>
        ''' <param name="version$">the workspace format version to use. NULL specifies the current default format. The version used from R 0.99.0 to R 1.3.1 was version 1. The default format as from R 1.4.0 is version 2.</param>
        ''' <param name="envir$">environment to search for objects to be saved.</param>
        ''' <param name="compress$">logical or character string specifying whether saving to a named file is to use compression. TRUE corresponds to gzip compression, and character strings "gzip", "bzip2" or "xz" specify the type of compression. Ignored when file is a connection and for workspace format version 1.</param>
        ''' <param name="compression_level$">integer: the level of compression to be used. Defaults to 6 for gzip compression and to 9 for bzip2 or xz compression.</param>
        ''' <param name="eval_promises">logical: should objects which are promises be forced before saving?</param>
        ''' <param name="precheck">logical: should the existence of the objects be checked before starting to save (and in particular before opening the file/connection)? Does not apply to version 1 saves.</param>
        ''' <remarks>
        ''' The names of the objects specified either as symbols (or character strings) in ... or as a character vector in list are used to look up the objects from environment envir. By default promises are evaluated, but if eval.promises = FALSE promises are saved (together with their evaluation environments). (Promises embedded in objects are always saved unevaluated.)
        ''' All R platforms use the XDR (bigendian) representation Of C ints And doubles In binary save-d files, And these are portable across all R platforms.
        ''' ASCII saves used To be useful For moving data between platforms but are now mainly Of historical interest. They can be more compact than binary saves where compression Is Not used, but are almost always slower To both read And write: binary saves compress much better than ASCII ones. Further, Decimal ASCII saves may Not restore Double/complex values exactly, And what value Is restored may depend On the R platform.
        ''' Default values For the ascii, compress, safe And version arguments can be modified With the "save.defaults" Option (used both by save And save.image), see also the 'Examples’ section. If a "save.image.defaults" option is set it is used in preference to "save.defaults" for function save.image (which allows this to have different defaults). In addition, compression_level can be part of the "save.defaults" option.
        ''' A connection that Is Not already open will be opened In mode "wb". Supplying a connection which Is open And Not In binary mode gives an Error.
        ''' 
        ''' ###### Compression
        ''' Large files can be reduced considerably In size by compression. A particular 46MB R Object was saved As 35MB without compression In 2 seconds, 22MB With gzip compression In 8 secs, 19MB With bzip2 compression In 13 secs And 9.4MB With xz compression In 40 secs. The load times were 1.3, 2.8, 5.5 And 5.7 seconds respectively. These results are indicative, but the relative performances Do depend On the actual file: xz compressed unusually well here.
        ''' It Is possible to compress later (with gzip, bzip2 Or xz) a file saved with compress = FALSE: the effect Is the same As saving With compression. Also, a saved file can be uncompressed And re-compressed under a different compression scheme (And see resaveRdaFiles For a way To Do so from within R).
        ''' </remarks>
        Public Sub save(objects As IEnumerable(Of String),
                        file$,
                        Optional ascii As Boolean = False,
                        Optional version$ = "NULL",
                        Optional envir$ = "parent.frame()",
                        Optional compress As Boolean = True,
                        Optional compression_level% = 6,
                        Optional eval_promises As Boolean = True,
                        Optional precheck As Boolean = True)
            SyncLock R
                With R
                    .call = $"save({objects.JoinBy(", ")}, 
     file = {Rstring(file.UnixPath)},
     ascii = {ascii.λ}, version = {version}, envir = {envir},
     compress = {compress.λ}, compression_level = {compression_level},
     eval.promises = {eval_promises.λ}, precheck = {precheck.λ})"
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' vector produces a vector of the given length and mode.
        ''' </summary>
        ''' <param name="mode$"></param>
        ''' <param name="length%"></param>
        ''' <returns></returns>
        Public Function vector(Optional mode$ = "logical", Optional length% = 0) As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- vector(mode = {Rstring(mode)}, length = {length})"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' Combine Values into a Vector or List
        ''' 
        ''' This is a generic function which combines its arguments.
        ''' The Default method combines its arguments To form a vector. All arguments are coerced To a common type which Is the type Of the returned value, And all attributes except names are removed.
        ''' 
        ''' (这个函数是针对于R对象而言的，不会将<paramref name="list"/>之中的值转义为字符串)
        ''' </summary>
        ''' <param name="list">objects to be concatenated.</param>
        ''' <param name="recursive">logical. If recursive = TRUE, the function recursively descends through lists (and pairlists) combining all their elements into a vector.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The output type is determined from the highest type of the components in the hierarchy NULL &lt; raw &lt; logical &lt; integer &lt; double &lt; complex &lt; character &lt; list &lt; expression. Pairlists are treated as lists, but non-vector components (such names and calls) are treated as one-element lists which cannot be unlisted even if recursive = TRUE.
        ''' c Is sometimes used for its side effect of removing attributes except names, for example to turn an array into a vector. as.vector Is a more intuitive way to do this, but also drops names. Note too that methods other than the default are Not required to do this (And they will almost certainly preserve a class attribute).
        ''' This Is a primitive function.
        ''' </remarks>
        Public Function c(list As IEnumerable(Of String), Optional recursive As Boolean = False) As String
            Dim out As String = App.NextTempName
            Call $"{out} <- c({list.JoinBy(", ")}, recursive = {recursive.λ})".__call
            Return out
        End Function

        Public Function c(Of T)(list As IEnumerable(Of T), Optional recursive As Boolean = False) As String
            Return base.c(list.SafeQuery.Select(AddressOf Scripting.CStrSafe), recursive:=recursive)
        End Function

        ''' <summary>
        ''' Combine Values into a Vector or List
        ''' 
        ''' This is a generic function which combines its arguments.
        ''' The Default method combines its arguments To form a vector. All arguments are coerced To a common type which Is the type Of the returned value, And all attributes except names are removed.
        ''' </summary>
        ''' <param name="list">objects to be concatenated.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The output type is determined from the highest type of the components in the hierarchy NULL &lt; raw &lt; logical &lt; integer &lt; double &lt; complex &lt; character &lt; list &lt; expression. Pairlists are treated as lists, but non-vector components (such names and calls) are treated as one-element lists which cannot be unlisted even if recursive = TRUE.
        ''' c Is sometimes used for its side effect of removing attributes except names, for example to turn an array into a vector. as.vector Is a more intuitive way to do this, but also drops names. Note too that methods other than the default are Not required to do this (And they will almost certainly preserve a class attribute).
        ''' This Is a primitive function.
        ''' </remarks>
        Public Function c(ParamArray list As String()) As String
            Return c(list, recursive:=False)
        End Function

        ''' <summary>
        ''' 默认为生成字符串数组，这个函数是针对于R字符串而言的，<paramref name="list"/>参数之中的字符串的值都会被转义为字符串
        ''' </summary>
        ''' <param name="list"></param>
        ''' <param name="stringVector"></param>
        ''' <returns></returns>
        Public Function c(list As String(), Optional stringVector As Boolean = True) As String
            If stringVector Then
                Return c(list.Select(AddressOf Rstring), recursive:=False)
            Else
                Return c(list, recursive:=False)
            End If
        End Function

        ''' <summary>
        ''' Functions to construct, coerce and check for both kinds of R lists.
        ''' (实际上这个函数就是相当于创建了一个空的<see cref="Object"/>对象)
        ''' </summary>
        ''' <param name="objects$">objects, possibly named.(对象的名称列表)</param>
        ''' <returns></returns>
        Public Function list(ParamArray objects$()) As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- list({objects.JoinBy(", ")})"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' Loading/Attaching and Listing of Packages, library and require load and attach add-on packages.
        ''' Load a available R package which was installed in the R system.(加载一个可用的R包)
        ''' </summary>
        ''' <param name="package">the name of a package, given as a name or literal character string, or a character string, depending on whether character.only is FALSE (default) or TRUE).</param>
        ''' <param name="help">the name of a package, given as a name or literal character string, or a character string, depending on whether character.only is FALSE (default) or TRUE).</param>
        ''' <param name="pos">the position on the search list at which to attach the loaded namespace. Can also be the name of a position on the current search list as given by search().</param>
        ''' <param name="libloc">a character vector describing the location of R library trees to search through, or NULL. The default value of NULL corresponds to all libraries currently known to .libPaths(). Non-existent library trees are silently ignored.</param>
        ''' <param name="characterOnly">a logical indicating whether package or help can be assumed to be character strings.</param>
        ''' <param name="logicalReturn">logical. If it is TRUE, FALSE or TRUE is returned to indicate success.</param>
        ''' <param name="warnConflicts">logical. If TRUE, warnings are printed about conflicts from attaching the new package. A conflict is a function masking a function, or a non-function masking a non-function.</param>
        ''' <param name="quietly">a logical. If TRUE, no message confirming package attaching is printed, and most often, no errors/warnings are printed if package attaching fails.</param>
        ''' <param name="verbose">a logical. If TRUE, additional diagnostics are printed.</param>
        ''' <returns></returns>
        Public Function library(package As String,
                                Optional help As String = NULL,
                                Optional pos As Integer = 2,
                                Optional libloc As String = NULL,
                                Optional characterOnly As Boolean = False,
                                Optional logicalReturn As Boolean = False,
                                Optional warnConflicts As Boolean = True,
                                Optional quietly As Boolean = False,
                                Optional verbose As String = packages.base.getOption.verbose)
            Dim out As SymbolicExpression =
                $"library({package}, {help}, pos = {pos}, lib.loc = {libloc},
                           character.only = {characterOnly}, logical.return = {logicalReturn.λ},
                           warn.conflicts = {warnConflicts.λ}, quietly = {quietly.λ},
                           verbose = {verbose})".__call
        End Function

        ''' <summary>
        ''' Loading/Attaching and Listing of Packages
        ''' </summary>
        ''' <param name="package">the name Of a package, given As a name Or literal character String, Or a character String, 
        ''' depending On whether character.only Is False (Default) Or True).</param>
        ''' <param name="libloc">a character vector describing the location Of R library trees To search through, Or NULL. 
        ''' The Default value Of NULL corresponds To all libraries currently known To .libPaths(). Non-existent library 
        ''' trees are silently ignored.</param>
        ''' <param name="quietly">a logical. If TRUE, no message confirming package attaching is printed, and most often, 
        ''' no errors/warnings are printed if package attaching fails.</param>
        ''' <param name="warnConflicts">logical. If TRUE, warnings are printed about conflicts from attaching the new package. 
        ''' A conflict is a function masking a function, or a non-function masking a non-function.</param>
        ''' <param name="characterOnly">a logical indicating whether package or help can be assumed to be character strings.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' library and require can only load/attach an installed package, and this is detected by having a ‘DESCRIPTION’ 
        ''' file containing a Built: field.
        '''
        ''' Under Unix-alikes, the code checks that the package was installed under a similar operating system As given 
        ''' by R.version$platform (the canonical name Of the platform under which R was compiled), provided it contains 
        ''' compiled code. Packages which Do Not contain compiled code can be Shared between Unix-alikes, but Not To 
        ''' other OSes because Of potential problems With line endings And OS-specific help files. If Sub-architectures 
        ''' are used, the OS similarity Is Not checked since the OS used To build may differ (e.g. i386-pc-linux-gnu 
        ''' code can be built On an x86_64-unknown-linux-gnu OS).
        '''
        ''' The package name given To library And require must match the name given In the package's ‘DESCRIPTION’ file exactly, 
        ''' even on case-insensitive file systems such as are common on Windows and OS X.
        ''' </remarks>
        Public Function require(package As String,
                                Optional libloc As String = NULL,
                                Optional quietly As Boolean = False,
                                Optional warnConflicts As Boolean = True,
                                Optional characterOnly As Boolean = False) As Boolean
            Return $"require({package},
                        lib.loc        = {libloc},
                        quietly        = {quietly.λ},
                        warn.conflicts = {warnConflicts.λ},
                        character.only = {characterOnly.λ})".__call.AsBoolean
        End Function

        ''' <summary>
        ''' Data Frames
        ''' 
        ''' This function creates data frames, tightly coupled collections of variables which share many of the properties of matrices and of lists, used as the fundamental data structure by most of R's modeling software.
        ''' </summary>
        ''' <param name="x">
        ''' these arguments are Of either the form value Or tag = value. Component names are created based On the tag (If present) Or the deparsed argument itself.
        ''' (其实在这里是R里面的对象的名称的列表)
        ''' </param>
        ''' <param name="rowNames">NULL or a single integer or character string specifying a column to be used as row names, or a character or integer vector giving the row names for the data frame.</param>
        ''' <param name="checkRows">If True Then the rows are checked For consistency Of length And names.</param>
        ''' <param name="checkNames">logical. If TRUE then the names of the variables in the data frame are checked to ensure that they are syntactically valid variable names and are not duplicated. If necessary they are adjusted (by make.names) so that they are.</param>
        ''' <param name="stringsAsFactors">logical: should character vectors be converted To factors? The 'factory-fresh’ default is TRUE, but this can be changed by setting options(stringsAsFactors = FALSE).</param>
        ''' <returns>
        ''' A data frame, a matrix-like structure whose columns may be of differing types (numeric, logical, factor and character and so on).
        '''
        ''' How the names Of the data frame are created Is complex, And the rest Of this paragraph Is only the basic story. 
        ''' If the arguments are all named And simple objects (Not lists, matrices Of data frames) Then the argument names 
        ''' give the column names. For an unnamed simple argument, a deparsed version Of the argument Is used As the name 
        ''' (With an enclosing I(...) removed). For a named matrix/list/data frame argument With more than one named column, 
        ''' the names Of the columns are the name Of the argument followed by a dot And the column name inside the argument: 
        ''' If the argument Is unnamed, the argument's column names are used. For a named or unnamed matrix/list/data frame 
        ''' argument that contains a single column, the column name in the result is the column name in the argument. 
        ''' Finally, the names are adjusted to be unique and syntactically valid unless check.names = FALSE.
        ''' </returns>
        ''' <remarks>
        ''' A data frame is a list of variables of the same number of rows with unique row names, given class "data.frame". If no variables are included, the row names determine the number of rows.
        '''
        ''' The column names should be non-empty, And attempts To use empty names will have unsupported results. Duplicate column names are allowed, but you need To use check.names = False For data.frame To generate such a data frame. However, Not all operations On data frames will preserve duplicated column names: For example matrix-Like subsetting will force column names in the result To be unique.
        '''
        ''' ``data.frame`` converts each of its arguments to a data frame by calling as.data.frame(optional = TRUE). As that Is a generic function, methods can be written to change the behaviour of arguments according to their classes: R comes With many such methods. Character variables passed To data.frame are converted To factor columns unless Protected by I Or argument stringsAsFactors Is False. If a list Or data frame Or matrix Is passed To data.frame it Is As If Each component Or column had been passed As a separate argument (except For matrices Of Class "model.matrix" And those Protected by I).
        '''
        ''' Objects passed To data.frame should have the same number Of rows, but atomic vectors (see Is.vector), factors And character vectors Protected by I will be recycled a whole number Of times If necessary (including As elements Of list arguments).
        '''
        ''' If row names are Not supplied In the Call To data.frame, the row names are taken from the first component that has suitable names, For example a named vector Or a matrix With rownames Or a data frame. (If that component Is subsequently recycled, the names are discarded With a warning.) If row.names was supplied As NULL Or no suitable component was found the row names are the Integer sequence starting at one (And such row names are considered To be 'automatic’, and not preserved by as.matrix).
        '''
        ''' If row names are supplied Of length one And the data frame has a Single row, the row.names Is taken To specify the row names And Not a column (by name Or number).
        '''
        ''' Names are removed from vector inputs Not Protected by I.
        '''
        ''' Default.stringsAsFactors Is a utility that takes getOption("stringsAsFactors") And ensures the result Is TRUE Or FALSE (Or throws an error if the value Is Not NULL).
        ''' </remarks>
        Public Function dataframe(x As IEnumerable(Of String),
                                  Optional rowNames As String() = Nothing,
                                  Optional checkRows As Boolean = False,
                                  Optional checkNames As Boolean = True,
                                  Optional stringsAsFactors As String = "default.stringsAsFactors()") As String

            Dim out As String = App.NextTempName

            Call $"{out} <- data.frame({x.JoinBy(", ")}, row.names = {rowNames}, check.rows = {checkRows},
           check.names = {checkNames},
           stringsAsFactors = {stringsAsFactors})".__call

            Return out
        End Function

        ''' <summary>
        ''' **Matrices**
        ''' 
        ''' + ``matrix`` creates a matrix from the given set of values.
        ''' + ``as.matrix`` attempts to turn its argument into a matrix.
        ''' + ``is.matrix`` tests if its argument Is a (strict) matrix.
        ''' </summary>
        ''' <param name="data">an optional data vector (including a list Or expression vector). Non-atomic classed R objects are coerced by as.vector And all attributes discarded.</param>
        ''' <param name="nrow">the desired number of rows.</param>
        ''' <param name="ncol">the desired number of columns.</param>
        ''' <param name="byrow">logical. If FALSE (the default) the matrix is filled by columns, otherwise the matrix is filled by rows.</param>
        ''' <param name="dimnames">A dimnames attribute For the matrix: NULL Or a list of length 2 giving the row And column names respectively. 
        ''' An empty list Is treated as NULL, And a list of length one as row names. The list can be named, 
        ''' And the list names will be used as names for the dimensions.</param>
        ''' <remarks>
        ''' If one of nrow or ncol is not given, an attempt is made to infer it from the length of data and the other parameter. If neither is given, a one-column matrix is returned.
        ''' If there are too few elements In data To fill the matrix, Then the elements In data are recycled. If data has length zero, NA Of an appropriate type Is used For atomic vectors (0 For raw vectors) And NULL For lists.
        ''' Is.matrix returns TRUE if x Is a vector And has a "dim" attribute of length 2) And FALSE otherwise. Note that a data.frame Is Not a matrix by this test. The function Is generic: you can write methods To handle specific classes Of objects, see InternalMethods.
        ''' as.matrix Is a generic function. The method for data frames will return a character matrix if there Is only atomic columns And any non-(numeric/logical/complex) column, applying as.vector to factors And format to other non-character columns. 
        ''' Otherwise, the usual coercion hierarchy (``logical &lt; integer &lt; double &lt; complex``) will be used, e.g., all-logical data frames will be coerced to a logical matrix, mixed logical-integer will give a integer matrix, etc.
        ''' The Default method For As.matrix calls As.vector(x), And hence e.g. coerces factors To character vectors.
        ''' When coercing a vector, it produces a one-column matrix, And promotes the names (if any) of the vector to the rownames of the matrix.
        ''' Is.matrix Is a primitive function.
        ''' The print method For a matrix gives a rectangular layout With dimnames Or indices. For a list matrix, the entries Of length Not one are printed In the form Integer,7 indicating the type And length.
        ''' </remarks>
        Public Function matrix(data As String,
                               Optional nrow As Integer = -1,
                               Optional ncol As Integer = -1,
                               Optional byrow As Boolean = False,
                               Optional dimnames As String = NULL) As String
            Dim x As var

            If nrow = -1 Then
                x = $"matrix({data}, ncol={ncol}, byrow={byrow.λ}, dimnames={dimnames})"
            ElseIf ncol = -1 Then
                x = $"matrix({data}, nrow={nrow}, byrow={byrow.λ}, dimnames={dimnames})"
            Else
                x = $"matrix({data}, nrow={nrow}, ncol={ncol}, byrow={byrow.λ}, dimnames={dimnames})"
            End If

            Return x
        End Function

        ''' <summary>
        ''' **Matrices**
        ''' 
        ''' + ``matrix`` creates a matrix from the given set of values.
        ''' + ``as.matrix`` attempts to turn its argument into a matrix.
        ''' + ``is.matrix`` tests if its argument Is a (strict) matrix.
        ''' </summary>
        ''' <param name="data">an optional data vector (including a list Or expression vector). Non-atomic classed R objects are coerced by as.vector And all attributes discarded.</param>
        ''' <param name="nrow">the desired number of rows.</param>
        ''' <param name="ncol">the desired number of columns.</param>
        ''' <param name="byrow">logical. If FALSE (the default) the matrix is filled by columns, otherwise the matrix is filled by rows.</param>
        ''' <param name="dimnames">A dimnames attribute For the matrix: NULL Or a list of length 2 giving the row And column names respectively. 
        ''' An empty list Is treated as NULL, And a list of length one as row names. The list can be named, 
        ''' And the list names will be used as names for the dimensions.</param>
        ''' <returns>函数返回在R之中的一个临时变量名字符串，可以通过这个变量名来引用R之中的这个matrix</returns>
        Public Function matrix(Of T)(data As IEnumerable(Of T),
                                     Optional nrow As Integer = -1,
                                     Optional ncol As Integer = -1,
                                     Optional byrow As Boolean = False,
                                     Optional dimnames As String = NULL) As String
            Dim strings$() = data.ToArray(AddressOf Scripting.ToString)
            Dim vec As String = c(strings, recursive:=False)
            Return matrix(vec, nrow, ncol, byrow, dimnames)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="args"></param>
        ''' <param name="[call]"></param>
        ''' <param name="immediate"></param>
        ''' <param name="noBreaks"></param>
        ''' <param name="domain"></param>
        ''' <returns></returns>
        Public Function warning(Optional args As IEnumerable(Of String) = Nothing,
                                Optional [call] As Boolean = True,
                                Optional immediate As Boolean = False,
                                Optional noBreaks As Boolean = False,
                                Optional domain As String = NULL) As String()
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="expr"></param>
        Public Sub suppressWarnings(expr As String)

        End Sub

        ''' <summary>
        ''' summary is a generic function used to produce result summaries of the results of various model fitting functions. 
        ''' The function invokes particular methods which depend on the class of the first argument.
        ''' </summary>
        ''' <param name="object$">an object for which a summary is desired.</param>
        ''' <returns></returns>
        Public Function summary(object$) As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- summary({[object]})"
                End With
            End SyncLock

            Return var
        End Function
    End Module
End Namespace
