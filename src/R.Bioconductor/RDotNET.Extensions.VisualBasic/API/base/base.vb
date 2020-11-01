#Region "Microsoft.VisualBasic::3d23c8db15f4f4086e137f7fec521fb3, RDotNET.Extensions.VisualBasic\API\base\base.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
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



' /********************************************************************************/

' Summaries:

'     Module base
' 
'         Properties: colnames, dimnames, length, names, rownames
' 
'         Function: argumentExpression, (+5 Overloads) c, cbind, (+2 Overloads) dataframe, exists
'                   (+2 Overloads) lapply, library, (+4 Overloads) list, load, log2
'                   ls, (+2 Overloads) matrix, order, parameterValueAssign, rbind
'                   rep, require, save, solve, sum
'                   summary, vector, warning
' 
'         Sub: __setNames, gc, rm, save, source
'              suppressWarnings
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API

    ''' <summary>
    ''' The R Base Package
    ''' 
    ''' This package contains the basic functions which let R function as a language: arithmetic, input/output, basic programming support, etc. 
    ''' Its contents are available through inheritance from any environment.
    ''' 
    ''' For a complete list of functions, use ``library(help = "base")``.
    ''' </summary>
    ''' 
    <Package("Rinterop.base")>
    Public Module base

        ''' <summary>
        ''' Assign a Value to a Name
        ''' 
        ''' Assign a value to a name in an environment.
        ''' </summary>
        ''' <param name="x">a variable name, given as a character string. No coercion is done, 
        ''' and the first element of a character vector of length greater than one will be 
        ''' used, with a warning.</param>
        ''' <param name="value">a value to be assigned to x.</param>
        ''' <param name="pos">where to do the assignment. By default, assigns into the current 
        ''' environment. See ‘Details’ for other possibilities.</param>
        ''' <param name="envir">the environment to use. See ‘Details’.</param>
        ''' <param name="inherits">should the enclosing frames of the environment be inspected?
        ''' </param>
        ''' <param name="immediate">an ignored compatibility feature.</param>
        ''' <returns>
        ''' This function is invoked for its side effect, which is assigning value to the 
        ''' variable x. If no envir is specified, then the assignment takes place in the 
        ''' currently active environment.
        '''
        ''' If inherits is TRUE, enclosing environments of the supplied environment are searched 
        ''' until the variable x is encountered. The value is then assigned in the environment 
        ''' in which the variable is encountered (provided that the binding is not locked: see 
        ''' lockBinding: if it is, an error is signaled). If the symbol is not encountered then 
        ''' assignment takes place in the user's workspace (the global environment).
        '''
        ''' If inherits is FALSE, assignment takes place in the initial frame of envir, unless 
        ''' an existing binding is locked or there is no existing binding and the environment 
        ''' is locked (when an error is signaled).
        ''' </returns>
        ''' <remarks>
        ''' There are no restrictions on the name given as x: it can be a non-syntactic name 
        ''' (see make.names).
        '''
        ''' The pos argument can specify the environment in which to assign the object in any 
        ''' of several ways: as -1 (the default), as a positive integer (the position in the 
        ''' search list); as the character string name of an element in the search list; or as 
        ''' an environment (including using sys.frame to access the currently active function 
        ''' calls). The envir argument is an alternative way to specify an environment, but 
        ''' is primarily for back compatibility.
        '''
        ''' assign does not dispatch assignment methods, so it cannot be used to set elements 
        ''' of vectors, names, attributes, etc.
        '''
        ''' Note that assignment to an attached list or data frame changes the attached copy 
        ''' and not the original object: see attach and with.
        ''' </remarks>
        <ExportAPI("assign")>
        Public Function assign(x$, value$,
                               Optional pos% = -1,
                               Optional envir$ = "as.environment({$pos})",
                               Optional [inherits] As Boolean = False,
                               Optional immediate As Boolean = True) As String
            SyncLock R
                With R
                    .call = $"assign({x.Rstring}, {value}, pos = {pos}, envir = {envir.Replace("{$pos}", pos)},
       inherits = {[inherits].λ}, immediate = {immediate.λ});"
                End With
            End SyncLock

            Return x
        End Function

        Public Function log2(vector As IEnumerable(Of Double)) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- log2({c(vector)});"
                End With
            End SyncLock

            Return var
        End Function

        Public Function order(x$, Optional nalast As Boolean = True, Optional decreasing As Boolean = False, Optional method$ = "shell") As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- order({x}, na.last = {nalast.λ}, decreasing = {decreasing.λ}, method = {Rstring(method)});"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' Solve a System of Equations
        ''' 
        ''' This generic function solves the equation a %*% x = b for x, where b can be either a 
        ''' vector or a matrix.
        ''' </summary>
        ''' <param name="a$">
        ''' a square numeric or complex matrix containing the coefficients of the linear system. Logical matrices are coerced to numeric.
        ''' </param>
        ''' <param name="b$">
        ''' a numeric or complex vector or matrix giving the right-hand side(s) of the linear system. If missing, b is taken to be an 
        ''' identity matrix and solve will return the inverse of a</param>
        ''' <param name="arguments">further arguments passed to or from other methods</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' a or b can be complex, but this uses double complex arithmetic which might not be available on all platforms.
        ''' The row and column names of the result are taken from the column names of a and of b respectively. If b is missing the column 
        ''' names of the result are the row names of a. No check is made that the column names of a and the row names of b are equal.
        ''' For back-compatibility a can be a (real) QR decomposition, although qr.solve should be called in that case. qr.solve can handle 
        ''' non-square systems.
        ''' Unsuccessful results from the underlying LAPACK code will result in an error giving a positive error code: these can only be 
        ''' interpreted by detailed study of the FORTRAN code.
        ''' </remarks>
        Public Function solve(a$, Optional b$ = Nothing, Optional arguments As Dictionary(Of String, String) = Nothing) As String
            Dim var$ = RDotNetGC.Allocate
            Dim args = arguments _
                .SafeQuery _
                .Select(Function(v) $"{v.Key} = {v.Value}") _
                .JoinBy(", ")
            Dim params$

            SyncLock R
                With R
                    If b.StringEmpty Then
                        params = {a, args}.JoinBy(", ")
                    Else
                        params = {a, b, args}.JoinBy(", ")
                    End If

                    .call = $"{var} <- solve({params});"
                End With
            End SyncLock

            Return var
        End Function


        ''' <summary>
        ''' The R engine execute a R script. source causes R to accept its input from the named file or URL or connection.
        ''' Input is read and parsed from that file until the end of the file is reached, then the parsed expressions are
        ''' evaluated sequentially in the chosen environment.
        ''' (R引擎执行文件系统之中的一个R脚本)
        ''' </summary>
        ''' <param name="file">a connection Or a character String giving the pathname Of the file Or URL To read from. ""
        ''' indicates the connection stdin().
        ''' </param>
        ''' <param name="local">TRUE, FALSE or an environment, determining where the parsed expressions are evaluated.
        ''' FALSE (the default) corresponds to the user's workspace (the global environment) and TRUE to the environment
        ''' from which source is called.</param>
        ''' <param name="echo">logical; if TRUE, each expression is printed after parsing, before evaluation.</param>
        ''' <param name="printEval">logical; if TRUE, the result of eval(i) is printed for each expression i; defaults to the value of echo.</param>
        ''' <param name="verbose">if TRUE, more diagnostics (than just echo = TRUE) are printed during parsing and evaluation of input,
        ''' including extra info for each expression.</param>
        ''' <param name="promptEcho">character; gives the prompt to be used if echo = TRUE.</param>
        ''' <param name="maxDeparseLength">integer; is used only if echo is TRUE and gives the maximal number of characters output for
        ''' the deparse of a single expression.</param>
        ''' <param name="chdir">logical; if TRUE And file Is a pathname, the R working directory Is temporarily changed to the
        ''' directory containing file for evaluating.</param>
        ''' <param name="encoding">character vector. The encoding(s) to be assumed when file is a character string: see file.
        ''' A possible value is "unknown" when the encoding is guessed: see the ‘Encodings’ section.</param>
        ''' <param name="continueEcho">character; gives the prompt to use on continuation lines if echo = TRUE.</param>
        ''' <param name="skipEcho">integer; how many comment lines at the start of the file to skip if echo = TRUE.</param>
        ''' <param name="keepSource">logical: should the source formatting be retained When echoing expressions, If possible?</param>
        ''' <remarks>
        ''' Note that running code via source differs in a few respects from entering it at the R command line. Since expressions are not executed
        ''' at the top level, auto-printing is not done. So you will need to include explicit print calls for things you want to be printed
        ''' (and remember that this includes plotting by lattice, FAQ Q7.22). Since the complete file is parsed before any of it is run, syntax
        ''' errors result in none of the code being run. If an error occurs in running a syntactically correct script, anything assigned into the
        ''' workspace by code that has been run will be kept (just as from the command line), but diagnostic information such as traceback() will
        ''' contain additional calls to withVisible.
        '''
        ''' All versions Of R accept input from a connection With End Of line marked by LF (As used On Unix), CRLF (As used On DOS/Windows) Or CR
        ''' (As used On classic Mac OS) And map this To newline. The final line can be incomplete, that Is missing the final End-Of-line marker.
        '''
        ''' If keep.source Is True(the Default In interactive use), the source Of functions Is kept so they can be listed exactly As input.
        '''
        ''' Unlike input from a console, lines In the file Or On a connection can contain an unlimited number Of characters.
        '''
        ''' When skip.echo > 0, that many comment lines at the start of the file will Not be echoed. This does Not affect the execution of the code at all.
        ''' If there are executable lines within the first skip.echo lines, echoing will start with the first of them.
        '''
        ''' If echo Is True And a deparsed expression exceeds max.deparse.length, that many characters are output followed by .... [TRUNCATED] .
        '''
        ''' [Encodings]
        ''' By Default the input Is read And parsed In the current encoding Of the R session. This Is usually what it required, but occasionally re-encoding
        ''' Is needed, e.g. If a file from a UTF-8-Using system Is To be read On Windows (Or vice versa).
        '''
        ''' The rest Of this paragraph applies If file Is an actual filename Or URL (And Not "" nor a connection). If encoding = "unknown", an attempt Is
        ''' made To guess the encoding: the result Of localeToCharset() Is used As a guide. If encoding has two Or more elements, they are tried In turn
        ''' until the file/URL can be read without Error In the trial encoding. If an actual encoding Is specified (rather than the Default Or "unknown")
        ''' In a Latin-1 Or UTF-8 locale Then character strings In the result will be translated To the current encoding And marked As such (see Encoding).
        '''
        ''' If file Is a connection (including one specified by "", it Is Not possible To re-encode the input inside source, And so the encoding argument
        ''' Is just used To mark character strings In the parsed input In Latin-1 And UTF-8 locales: see parse.
        ''' </remarks>
        Public Sub source(file As String,
                          Optional local As Boolean = True,
                          Optional echo As Boolean = False,
                          Optional printEval As Boolean = False,
                          Optional verbose As Boolean = False,
                          Optional promptEcho As Boolean = False,
                          Optional maxDeparseLength As Integer = 150,
                          Optional chdir As Boolean = False,
                          Optional encoding As String = "unknown",
                          Optional continueEcho As Boolean = False,
                          Optional skipEcho As Integer = 0,
                          Optional keepSource As Boolean = False)
            SyncLock R
                With R
                    .call = $"source(""{If(local, UnixPath(file), file)}"", local = {Rbool(local)}, echo = {Rbool(echo)}, print.eval = {Rbool(printEval)},
                         verbose = {Rbool(verbose)},
                         prompt.echo = {Rbool(promptEcho)},
                         max.deparse.length = {maxDeparseLength}, chdir = {Rbool(chdir)},
                         encoding = {Rstring(encoding)},
                         continue.echo = {Rbool(continueEcho)},
                         skip.echo = {skipEcho}, 
                         keep.source = {Rbool(keepSource)});"
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' Replicate Elements of Vectors and Lists
        ''' </summary>
        ''' <param name="x$">
        ''' a vector (of any mode including a list) or a factor or (for rep only) a POSIXct or POSIXlt 
        ''' or Date object; or an S4 object containing such an object.
        ''' </param>
        ''' <param name="times%">
        ''' an integer-valued vector giving the (non-negative) number of times to repeat each element 
        ''' if of length length(x), or to repeat the whole vector if of length 1. Negative or NA values 
        ''' are an error. A double vector is accepted, other inputs being coerced to an integer or 
        ''' double vector.
        ''' </param>
        ''' <returns></returns>
        Public Function rep(x$, times%) As String
            SyncLock R
                With R
                    Dim var$ = RDotNetGC.Allocate

                    .call = $"{var} <- rep({x}, times = {times});"

                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' The Names of an Object
        ''' 
        ''' Functions to get or set the names of an object.
        ''' </summary>
        ''' <param name="x$">an R object.</param>
        ''' <value>
        ''' a character vector of up to the same length as x, or NULL.
        ''' 
        ''' For names, NULL or a character vector of the same length as x. (NULL is given if the object has no names, 
        ''' including for objects of types which cannot have names.) For an environment, the length is the number 
        ''' of objects in the environment but the order of the names is arbitrary.
        ''' For names&lt;-, the updated object. (Note that the value of names(x) &lt;- value is that of the assignment, 
        ''' value, not the return value from the left-hand side.)
        ''' </value>
        ''' <returns></returns>
        ''' <remarks>
        ''' names is a generic accessor function, and names &lt;- is a generic replacement function. 
        ''' The default methods get and set the "names" attribute of a vector (including a list) or pairlist.
        ''' For an environment env, names(env) gives the names of the corresponding list, i.e., 
        ''' names(as.list(env, all.names = TRUE)) which are also given by ls(env, all.names = TRUE, sorted = FALSE). 
        ''' If the environment is used as a hash table, names(env) are its “keys”.
        ''' If value is shorter than x, it is extended by character NAs to the length of x.
        ''' It is possible to update just part of the names attribute via the general rules: see the examples. 
        ''' This works because the expression there is evaluated as z &lt;- "names&lt;-"(z, "[&lt;-"(names(z), 3, "c2")).
        ''' The name "" is special: it is used to indicate that there is no name associated with an element of 
        ''' a (atomic or generic) vector. Subscripting by "" will match nothing (not even elements which have no name).
        ''' A name can be character NA, but such a name will never be matched and is likely to lead to confusion.
        ''' Both are primitive functions.
        ''' 
        ''' For vectors, the names are one of the attributes with restrictions on the possible values. 
        ''' For pairlists, the names are the tags and converted to and from a character vector.
        ''' For a one-dimensional array the names attribute really is dimnames[[1]].
        ''' Formally classed aka “S4” objects typically have slotNames() (and no names()).
        ''' </remarks>
        Public Property names(x$) As String()
            Get
                SyncLock R
                    With R
                        Dim s As SymbolicExpression = .Evaluate($"names({x})")
                        Dim namelist$() = s.ToStrings
                        Return namelist
                    End With
                End SyncLock
            End Get
            Set(value As String())
                Call x.__setNames(value, "names")
            End Set
        End Property

        ''' <summary>
        ''' Retrieve or set the dimnames of an object.
        ''' </summary>
        ''' <param name="x">an R object, for example a matrix, array or data frame.</param>
        ''' <returns>返回变量指针字符串</returns>
        Public Property dimnames(x As String) As String
            Get
                Dim var$ = RDotNetGC.Allocate

                SyncLock R
                    With R
                        .call = $"{var} <- dimnames({x});"
                    End With
                End SyncLock

                Return var
            End Get
            Set(value As String)
                SyncLock R
                    With R
                        .call = $"dimnames({x}) <- {value}"
                    End With
                End SyncLock
            End Set
        End Property

        ''' <summary>
        ''' Retrieve or set the row or column names of a matrix-like object.
        ''' </summary>
        ''' <param name="x$">a matrix-Like R Object, With at least two dimensions For colnames.</param>
        ''' <param name="doNULL">logical. If FALSE and names are NULL, names are created.</param>
        ''' <param name="prefix$">for created names.</param>
        ''' <returns>
        ''' a valid value for that component of dimnames(x). 
        ''' For a matrix or array this is either NULL or a character vector of non-zero length 
        ''' equal to the appropriate dimension.
        ''' </returns>
        Public Property colnames(x$, Optional doNULL As Boolean = True, Optional prefix$ = "col") As String()
            Get
                SyncLock R
                    With R
                        Dim s As SymbolicExpression = .Evaluate($"colnames({x}, do.NULL = {doNULL.λ}, prefix = {Rstring(prefix)})")
                        Dim namelist$() = s.ToStrings
                        Return namelist
                    End With
                End SyncLock
            End Get
            Set(value As String())
                Call x.__setNames(value, "colnames")
            End Set
        End Property

        ''' <summary>
        ''' Retrieve or set the row or column names of a matrix-like object.
        ''' </summary>
        ''' <param name="x$">a matrix-Like R Object, With at least two dimensions For colnames.</param>
        ''' <param name="doNULL">logical. If FALSE and names are NULL, names are created.</param>
        ''' <param name="prefix$">for created names.</param>
        ''' <returns>
        ''' a valid value for that component of dimnames(x). 
        ''' For a matrix or array this is either NULL or a character vector of non-zero length 
        ''' equal to the appropriate dimension.
        ''' </returns>
        Public Property rownames(x$, Optional doNULL As Boolean = True, Optional prefix$ = "col") As StringVector
            Get
                SyncLock R
                    With R
                        Dim s As SymbolicExpression = .Evaluate($"rownames({x}, do.NULL = {doNULL.λ}, prefix = {Rstring(prefix)})")
                        Dim namelist$() = s.ToStrings

                        Return namelist
                    End With
                End SyncLock
            End Get
            Set(value As StringVector)
                If value.IsSingle AndAlso base.exists(value.First) Then
                    SyncLock R
                        With R
                            value = .Evaluate(value.First) _
                                .AsCharacter _
                                .ToArray
                        End With
                    End SyncLock
                End If

                Call x.__setNames(value.ToArray, "rownames")
            End Set
        End Property

        ''' <summary>
        ''' Look for an R object of the given name and possibly return it
        ''' </summary>
        ''' <param name="x$">a variable name (given as a character string).</param>
        ''' <param name="where%">where to look for the object (see the details section); 
        ''' if omitted, the function will search as if the name of the object appeared 
        ''' unquoted in an expression.</param>
        ''' <param name="envir$">an alternative way to specify an environment to look in, 
        ''' but it is usually simpler to just use the where argument.</param>
        ''' <param name="frame$">a frame in the calling list. Equivalent to giving where 
        ''' as ``sys.frame(frame)``.</param>
        ''' <param name="mode$">the mode or type of object sought: see the ‘Details’ section.
        ''' </param>
        ''' <param name="[inherits]">should the enclosing frames of the environment be searched?
        ''' </param>
        ''' <returns>
        ''' Logical, true if and only if an object of the correct name and mode is found.
        ''' </returns>
        ''' <remarks>
        ''' The where argument can specify the environment in which to look for the 
        ''' object in any of several ways: as an integer (the position in the search 
        ''' list); as the character string name of an element in the search list; 
        ''' or as an environment (including using sys.frame to access the currently 
        ''' active function calls). The envir argument is an alternative way to specify 
        ''' an environment, but is primarily there for back compatibility.
        ''' This Function looks To see If the name x has a value bound To it In the 
        ''' specified environment. If Inherits Is True And a value Is Not found For 
        ''' x In the specified environment, the enclosing frames Of the environment 
        ''' are searched until the name x Is encountered. See environment And the 'R 
        ''' Language Definition’ manual for details about the structure of environments 
        ''' and their enclosures.
        ''' Warning: Inherits = TRUE Is the default behaviour for R but Not for S.
        ''' If mode Is specified Then only objects of that type are sought. The mode 
        ''' may specify one of the collections "numeric" And "function" (see mode): 
        ''' Any member Of the collection will suffice. (This Is True even If a member 
        ''' Of a collection Is specified, so for example mode = "special" will seek 
        ''' any type of function.)
        ''' </remarks>
        Public Function exists(x$,
                               Optional where% = -1,
                               Optional envir$ = "",
                               Optional frame$ = NULL,
                               Optional mode$ = "any",
                               Optional [inherits] As Boolean = True) As Boolean
            SyncLock R
                With R
                    Return .Evaluate(statement:=$"exists({Rstring(x)}, where = {where},  mode = {Rstring(mode)}, inherits = {[inherits].λ})") _
                           .AsLogical _
                           .First
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Template for invoke set names function in R language
        ''' </summary>
        ''' <param name="x$"></param>
        ''' <param name="names$"></param>
        ''' <param name="func$"></param>
        ''' <remarks>
        ''' The extractor functions try to do something sensible for any matrix-like object x. 
        ''' If the object has dimnames the first component is used as the row names, and the second 
        ''' component (if any) is used for the column names. For a data frame, rownames and colnames 
        ''' eventually call row.names and names respectively, but the latter are preferred.
        ''' 
        ''' If do.NULL Is FALSE, a character vector (of length NROW(x) Or NCOL(x)) Is returned in 
        ''' any case, prepending prefix to simple numbers, if there are no dimnames Or the 
        ''' corresponding component of the dimnames Is NULL.
        ''' 
        ''' The replacement methods For arrays/matrices coerce vector And factor values Of value 
        ''' To character, but Do Not dispatch methods For As.character.
        ''' 
        ''' For a data frame, value for rownames should be a character vector of non-duplicated 
        ''' And non-missing names (this Is enforced), And for colnames a character vector of 
        ''' (preferably) unique syntactically-valid names. In both cases, value will be coerced 
        ''' by as.character, And setting colnames will convert the row names To character.
        ''' </remarks>
        <Extension> Private Sub __setNames(x$, names$(), func$)
            Dim vector$ = c(names, stringVector:=True)

            SyncLock R
                With R
                    .call = $"{func}({x}) <- {vector};"
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' lapply returns a list of the same length as X, each element of which is the result of applying 
        ''' <paramref name="FUN"/> to the corresponding element of X.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="x">
        ''' a vector (atomic or list) or an expression object. Other objects (including classed objects) will 
        ''' be coerced by ``base::as.list``.
        ''' </param>
        ''' <param name="FUN">
        ''' the function to be applied to each element of X: see ‘Details’. In the case of functions like +, 
        ''' %*%, the function name must be backquoted or quoted.
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function lapply(Of T As INamedValue)(x As IEnumerable(Of T), FUN As Func(Of T, String), Optional autoGC As Boolean = False) As String
            Return lapply(x, Function(obj) obj.Key, FUN, autoGC)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function lapply(Of T)(x As IEnumerable(Of IGrouping(Of String, T)), FUN As Func(Of IEnumerable(Of T), String), Optional autoGC As Boolean = False) As String
            Return lapply(x, Function(group) group.Key, Function(group) FUN(group), autoGC)
        End Function

        ''' <summary>
        ''' lapply returns a list of the same length as X, each element of which is the result of applying 
        ''' <paramref name="FUN"/> to the corresponding element of X.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="x">
        ''' a vector (atomic or list) or an expression object. Other objects (including classed objects) will 
        ''' be coerced by ``base::as.list``.
        ''' </param>
        ''' <param name="FUN">
        ''' the function to be applied to each element of X: see ‘Details’. In the case of functions like +, 
        ''' %*%, the function name must be backquoted or quoted.
        ''' </param>
        ''' <returns></returns>
        Public Function lapply(Of T)(x As IEnumerable(Of T), key As Func(Of T, String), FUN As Func(Of T, String), Optional autoGC As Boolean = False) As String
            Dim list$ = base.list

            If autoGC Then
                Call RDotNetGC.Exclude(list)
            End If

            SyncLock R
                With R
                    Call x.DoEach(Sub(obj)
                                      .call = $"{list}[[""{key(obj)}""]] = {FUN(obj)}"

                                      If autoGC AndAlso RDotNetGC.numObjects > 10000 Then
                                          Call RDotNetGC.Do()
                                      End If
                                  End Sub)
                End With
            End SyncLock

            Return list
        End Function

        ''' <summary>
        ''' lapply on a dictionary list object.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="x"></param>
        ''' <param name="FUN"></param>
        ''' <returns></returns>
        Public Function lapply(Of T)(x As Dictionary(Of String, T), FUN As Func(Of T, String, String)) As String
            Return lapply(Of KeyValuePair(Of String, T))(
                x:=x,
                key:=Function(obj) obj.Key,
                FUN:=Function(tuple) FUN(tuple.Value, tuple.Key)
            )
        End Function

        ''' <summary>
        ''' ###### load {base}
        ''' 
        ''' Reload Saved Datasets, Reload datasets written with the function <see cref="save"/>.
        ''' 
        ''' (这个函数返回所加载的rda文件数据集之中的所存储的对象的列表)
        ''' </summary>
        ''' <param name="file$">
        ''' a (readable binary-mode) connection or a character string giving the name of the file to load (when tilde expansion is done).
        ''' (文件路径字符串不需要进行特殊处理，在函数这里已经会被自动处理了)
        ''' </param>
        ''' <param name="verbose">should item names be printed during loading?</param>
        ''' <returns>
        ''' load can load R objects saved in the current or any earlier format. It can read a compressed file (see save) directly from a file or from a suitable connection (including a call to url).
        ''' A not-open connection will be opened in mode "rb" and closed after use. Any connection other than a gzfile or gzcon connection will be wrapped in gzcon to allow compressed saves to be handled: note that this leaves the connection in an altered state (in particular, binary-only), and that it needs to be closed explicitly (it will not be garbage-collected).
        ''' Only R objects saved in the current format (used since R 1.4.0) can be read from a connection. If no input is available on a connection a warning will be given, but any input not in the current format will result in a error.
        ''' Loading from an earlier version will give a warning about the ‘magic number’: magic numbers 1971:1977 are from R &lt; 0.99.0, and RD[ABX]1 from R 0.99.0 to R 1.3.1. These are all obsolete, and you are strongly recommended to re-save such files in a current format.
        ''' The verbose argument is mainly intended for debugging. If it is TRUE, then as objects from the file are loaded, their names will be printed to the console. If verbose is set to an integer value greater than one, additional names corresponding to attributes and other parts of individual objects will also be printed. Larger values will print names to a greater depth.
        ''' Objects can be saved with references to namespaces, usually as part of the environment of a function or formula. As from R 3.1.0 such objects can be loaded even if the namespace is not available: it is replaced by a reference to the global environment with a warning. The warning identifies the first object with such a reference (but there may be more than one).
        ''' </returns>
        Public Function load(file$, Optional verbose As Boolean = False) As String()
            SyncLock R
                With R
                    Dim expr$ = $"load(file = {Rstring(file.UnixPath)}, verbose = {verbose.λ})"
                    Dim var$ = RDotNetGC.Allocate

                    .call = $"{var} <- {expr}"

                    Dim names$() = .Evaluate(var).ToStrings
                    Return names
                End With
            End SyncLock
        End Function

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
        ''' <param name="[inherits]">should the enclosing frames of the environment be inspected?</param>
        Public Sub rm(list$, Optional pos% = -1, Optional [inherits] As Boolean = False)
            SyncLock R
                With R
                    .call = $"rm(list = {list}, pos = {pos}, envir = as.environment({pos}), inherits = {[inherits].λ})"
                End With
            End SyncLock
        End Sub

        Public Sub gc()
            SyncLock R
                With R
                    .call = "gc();"
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
        ''' <param name="name$">which environment to use in listing the available objects. Defaults to the current environment. 
        ''' Although called name for back compatibility, in fact this argument can specify the environment in any form; see the 
        ''' ``Details`` section.</param>
        ''' <param name="pos$">
        ''' an alternative argument to name for specifying the environment as a position in the search list. Mostly there for 
        ''' back compatibility.
        ''' </param>
        ''' <param name="allnames">
        ''' a logical value. If TRUE, all object names are returned. If FALSE, names which begin with a . are omitted.
        ''' </param>
        ''' <param name="pattern$">
        ''' an optional regular expression. Only names matching pattern are returned. glob2rx can be used to convert wildcard 
        ''' patterns to regular expressions.
        ''' </param>
        ''' <param name="sorted">
        ''' logical indicating if the resulting character should be sorted alphabetically. Note that this is part of ``ls()``
        ''' may take most of the time.
        ''' </param>
        ''' <returns></returns>
        Public Function ls(Optional name$ = Nothing,
                           Optional pos$ = "-1L",
                           Optional allnames As Boolean = False,
                           Optional pattern$ = Nothing,
                           Optional sorted As Boolean = True) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    If name.StringEmpty Then
                        .call = $"{var} <- ls(pos = {pos}, envir = as.environment({pos}), all.names = {allnames.λ}, pattern = {pattern}, sorted = {sorted.λ});"
                    Else
                        .call = $"{var} <- ls({name}, pos = {pos}, envir = as.environment({pos}), all.names = {allnames.λ}, pattern = {pattern}, sorted = {sorted.λ});"
                    End If

                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' writes an external representation of R objects to the specified file. The objects can be read 
        ''' back from the file at a later date by using the function load or attach (or data in some cases).
        ''' (这个函数是安全的函数，假若文件夹不存在的话，这个函数会自动创建文件夹)
        ''' </summary>
        ''' <param name="objects">the names of the objects to be saved (as symbols or character strings).</param>
        ''' <param name="file$">a (writable binary-mode) connection or the name of the file where the data 
        ''' will be saved (when tilde expansion is done). Must be a file name for save.image or version = 1.</param>
        ''' <param name="ascii">if TRUE, an ASCII representation of the data is written. The default 
        ''' value of ascii is FALSE which leads to a binary file being written. If NA and version >= 2, a 
        ''' different ASCII representation is used which writes double/complex numbers as binary fractions.</param>
        ''' <param name="version$">the workspace format version to use. NULL specifies the current default format.
        ''' The version used from R 0.99.0 to R 1.3.1 was version 1. The default format as from R 1.4.0 is version 2.</param>
        ''' <param name="envir$">environment to search for objects to be saved.</param>
        ''' <param name="compress$">logical or character string specifying whether saving to a named file is 
        ''' to use compression. TRUE corresponds to gzip compression, and character strings "gzip", "bzip2" 
        ''' or "xz" specify the type of compression. Ignored when file is a connection and for workspace 
        ''' format version 1.</param>
        ''' <param name="compression_level$">integer: the level of compression to be used. Defaults to 6 
        ''' for gzip compression and to 9 for bzip2 or xz compression.</param>
        ''' <param name="eval_promises">logical: should objects which are promises be forced before saving?</param>
        ''' <param name="precheck">logical: should the existence of the objects be checked before starting 
        ''' to save (and in particular before opening the file/connection)? Does not apply to version 1 saves.</param>
        ''' <remarks>
        ''' The names of the objects specified either as symbols (or character strings) in ... or as a 
        ''' character vector in list are used to look up the objects from environment envir. By default 
        ''' promises are evaluated, but if eval.promises = FALSE promises are saved (together with their 
        ''' evaluation environments). (Promises embedded in objects are always saved unevaluated.)
        ''' All R platforms use the XDR (bigendian) representation Of C ints And doubles In binary saved 
        ''' files, And these are portable across all R platforms.
        ''' ASCII saves used To be useful For moving data between platforms but are now mainly Of historical 
        ''' interest. They can be more compact than binary saves where compression Is Not used, but are 
        ''' almost always slower To both read And write: binary saves compress much better than ASCII ones. 
        ''' Further, Decimal ASCII saves may Not restore Double/complex values exactly, And what value Is 
        ''' restored may depend On the R platform.
        ''' Default values For the ascii, compress, safe And version arguments can be modified With the 
        ''' "save.defaults" Option (used both by save And save.image), see also the 'Examples’ section. 
        ''' If a "save.image.defaults" option is set it is used in preference to "save.defaults" for function 
        ''' save.image (which allows this to have different defaults). In addition, compression_level can be 
        ''' part of the "save.defaults" option.
        ''' A connection that Is Not already open will be opened In mode "wb". Supplying a connection which 
        ''' Is open And Not In binary mode gives an Error.
        ''' 
        ''' ###### Compression
        ''' Large files can be reduced considerably In size by compression. A particular 46MB R Object was 
        ''' saved As 35MB without compression In 2 seconds, 22MB With gzip compression In 8 secs, 19MB With 
        ''' bzip2 compression In 13 secs And 9.4MB With xz compression In 40 secs. The load times were 1.3, 
        ''' 2.8, 5.5 And 5.7 seconds respectively. These results are indicative, but the relative performances 
        ''' Do depend On the actual file: xz compressed unusually well here.
        ''' It Is possible to compress later (with gzip, bzip2 Or xz) a file saved with compress = FALSE: the 
        ''' effect Is the same As saving With compression. Also, a saved file can be uncompressed And re-compressed 
        ''' under a different compression scheme (And see resaveRdaFiles For a way To Do so from within R).
        ''' </remarks>
        ''' 
        <ExportAPI("save")>
        Public Sub save(objects As String(),
                        file$,
                        Optional ascii As Boolean = False,
                        Optional version$ = "NULL",
                        Optional envir$ = "parent.frame()",
                        Optional compress As Boolean = True,
                        Optional compression_level% = 6,
                        Optional eval_promises As Boolean = True,
                        Optional precheck As Boolean = True)

            If file.DirectoryExists Then

                ' 2018-6-21
                ' 如果在指定的位置存在一个同名的文件夹，将会产生
                ' can not open the connection的错误
                '
                ' 在这里给出错误信息
                Throw New InvalidOperationException($"There is a directory which is located at ""{file}"", please delete it and then try again!")

            Else
                Call file.ParentPath.MkDIR
            End If

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

        Public Function save(vars As var(),
                             file$,
                             Optional ascii As Boolean = False,
                             Optional version$ = "NULL",
                             Optional envir$ = "parent.frame()",
                             Optional compress As Boolean = True,
                             Optional compression_level% = 6,
                             Optional eval_promises As Boolean = True,
                             Optional precheck As Boolean = True) As Boolean
            Try
                Call base.save(objects:=vars.Select(Function(v) v.name),
                               file:=file,
                               ascii:=ascii,
                               compress:=compress,
                               compression_level:=compression_level,
                               envir:=envir,
                               eval_promises:=eval_promises,
                               precheck:=precheck,
                               version:=version
                )
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.PrintException

                Return False
            End Try

            Return True
        End Function

        ''' <summary>
        ''' Serialization Interface for Single Objects
        ''' 
        ''' Functions to write a single R object to a file, and to restore it.
        ''' </summary>
        ''' <param name="object">R object to serialize.</param>
        ''' <param name="file">a connection or the name of the file where the 
        ''' R object is saved to or read from.</param>
        ''' <param name="ascii">a logical. If TRUE or NA, an ASCII representation 
        ''' is written; otherwise (default), a binary one is used. See the 
        ''' comments in the help for save.</param>
        ''' <param name="version">the workspace format version to use. NULL 
        ''' specifies the current default version (3). The only other supported 
        ''' value is 2, the default from R 1.4.0 to R 3.5.0.</param>
        ''' <param name="compress">a logical specifying whether saving to a named 
        ''' file is to use "gzip" compression, or one of "gzip", "bzip2" or "xz" 
        ''' to indicate the type of compression to be used. Ignored if file is a 
        ''' connection.</param>
        ''' <param name="refhook">a hook function for handling reference objects.
        ''' </param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("saveRDS")>
        Public Function saveRDS([object] As String,
                                Optional file$ = "",
                                Optional ascii As Boolean = False,
                                Optional version$ = "NULL",
                                Optional compress As Boolean = True,
                                Optional refhook$ = "NULL") As Boolean

            If file.DirectoryExists Then

                ' 2018-6-21
                ' 如果在指定的位置存在一个同名的文件夹，将会产生
                ' can not open the connection的错误
                '
                ' 在这里给出错误信息
                Throw New InvalidOperationException($"There is a directory which is located at ""{file}"", please delete it and then try again!")

            Else
                Call file.ParentPath.MkDIR
            End If

            SyncLock R
                With R
                    .call = $"saveRDS({[object]}, file = {Rstring(file.UnixPath)}, ascii = {ascii.λ}, version = {version},
        compress = {compress.λ}, refhook = {refhook});"
                End With
            End SyncLock

            Return True
        End Function

        ''' <summary>
        ''' vector produces a vector of the given length and mode.
        ''' </summary>
        ''' <param name="mode$"></param>
        ''' <param name="length%"></param>
        ''' <returns></returns>
        Public Function vector(Optional mode$ = "logical", Optional length% = 0) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- vector(mode = {Rstring(mode)}, length = {length})"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' 这个函数会自动将NAN,Inf等结果值转换为R之中的NA值
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function c(x As IEnumerable(Of Double)) As String
            Return base.c(list:=x _
                .SafeQuery _
                .Select(Function(d)
                            If d.IsNaNImaginary Then
                                Return "NA"
                            Else
                                Return CStr(d)
                            End If
                        End Function),
                recursive:=False
            )
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
            With list.SafeQuery.ToArray
                If .Length = 0 Then
                    Return "NULL"
                Else
                    SyncLock R
                        Dim out As var = NULL

                        ' 2018-11-22 似乎R的交互式解释器有一个最长表达式的限制
                        ' 所以在这里如果向量的申明表达式过长的画，会触发一个StackOverflow的错误
                        ' 但是如果直接将表达式保存为脚本文件，然后使用source执行，则并不会存在这个bug

                        ' 在这里为了避免出现这个问题，会需要将向量按照块进行切割，然后使用append进行合并
                        For Each block As String() In .Split(100)
                            Dim v$ = block.JoinBy(", ")

                            If recursive Then
                                v = $"c({v}, recursive = {CStr(recursive).ToUpper})"
                            Else
                                v = $"c({v})"
                            End If

                            R.call = $"{out} <- append({out}, {v});"
                        Next

                        ' Return RSystem.ref(out)
                        Return out
                    End SyncLock
                End If
            End With
        End Function

        ''' <summary>
        ''' This is a generic function which combines its arguments.
        ''' The Default method combines its arguments To form a vector. All arguments are coerced To a common type which Is the type Of the returned value, And all attributes except names are removed.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="list"></param>
        ''' <param name="recursive"></param>
        ''' <returns></returns>
        Public Function c(Of T)(list As IEnumerable(Of T), Optional recursive As Boolean = False) As String
            Return base.c(list.SafeQuery.Select(AddressOf CStrSafe), recursive:=recursive)
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
        Public Function c(ParamArray list As Boolean()) As String
            Return c(list.Select(Function(b) b.ToString.ToUpper), recursive:=False)
        End Function

        ''' <summary>
        ''' Execute a ``c()`` vector api and returns a tmp variable name.
        ''' (默认为生成字符串数组，这个函数是针对于R字符串而言的，
        ''' <paramref name="list"/>参数之中的字符串的值都会被转义为字符串)
        ''' </summary>
        ''' <param name="list">
        ''' 所需要生成集合的对象列表，这个参数所代表的含义会依据<paramref name="stringVector"/>的值而变化
        ''' </param>
        ''' <param name="stringVector">
        ''' 当这个参数为真的时候，则这个函数生成的是一个字符串向量，反之为False的时候，
        ''' 输入的<paramref name="list"/>将不会被转义，即输出由一系列变量所生成的一个集合
        ''' </param>
        ''' <returns></returns>
        Public Function c(list$(), Optional stringVector As Boolean = True) As String
            ' c() == NULL in R
            If list.IsNullOrEmpty Then
                Return NULL
            End If

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
        ''' <param name="args$">objects, possibly named.(对象的名称列表)</param>
        ''' <returns></returns>
        Public Function list(ParamArray args As ArgumentReference()) As String
            Dim var$ = RDotNetGC.Allocate
            Dim Rscript$

            SyncLock R
                With R
                    Rscript = $"{var} <- list({args.ArgumentExpression.JoinBy("," & vbCrLf)});"
                    .call = Rscript

                    ' 20200109 fix for bug of var not found
                    ' unsure....
                    If Not base.exists(var) Then
                        Call Console.WriteLine(Rscript)

                        .call = $"{var} <- list();"

                        For Each slot In args.Select(AddressOf ParameterValueAssign)
                            .call = $"{var}[['{slot.Name}']] <- {slot.Value};"
                        Next

                        ' debug
                        .call = $"str({var});"
                    End If
                End With
            End SyncLock

            Return var
        End Function

        Public Function list(obj As Dictionary(Of String, String)) As String
            SyncLock R
                With R
                    Dim var$ = base.list

                    For Each slot In obj
                        .call = $"{var}$'{slot.Key}' <- {slot.Value};"
                    Next

                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Create an empty list
        ''' </summary>
        ''' <returns></returns>
        Public Function list() As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- list();"
                End With
            End SyncLock

            Return var
        End Function

        Public Function list(ParamArray objects$()) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- list({objects.JoinBy(",")});"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' Get or set the length of vectors (including lists) and factors, 
        ''' and of any other R object for which a method has been defined.
        ''' </summary>
        ''' <param name="x$">an R object. For replacement, a vector or factor.</param>
        ''' <returns>a non-negative integer or double (which will be rounded down).</returns>
        Public Property length(x$) As Integer
            Get
                SyncLock R
                    With R
                        Return .Evaluate($"length({x})") _
                               .AsInteger _
                               .ToArray _
                               .First
                    End With
                End SyncLock
            End Get
            Set(value As Integer)
                SyncLock R
                    With R
                        .call = $"length({x}) <- {value};"
                    End With
                End SyncLock
            End Set
        End Property

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
                                Optional verbose As String = packages.base.getOption.verbose) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- library({package}, {help}, pos = {pos}, lib.loc = {libloc}, 
    character.only = {characterOnly}, logical.return = {logicalReturn.λ},
    warn.conflicts = {warnConflicts.λ}, quietly = {quietly.λ},
    verbose = {verbose})"
                End With
            End SyncLock

            Return var
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
        ''' ### Data Frames
        ''' 
        ''' This function creates data frames, tightly coupled collections of variables which share many of the properties of matrices and 
        ''' of lists, used as the fundamental data structure by most of R's modeling software.
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

            Dim var As String = RDotNetGC.Allocate
            Dim paramRowNames$

            If rowNames.IsNullOrEmpty Then
                paramRowNames = "NULL"
            Else
                paramRowNames = base.c(rowNames, stringVector:=True)
            End If

            Call $"{var} <- data.frame({x.JoinBy(", ")}, row.names = {paramRowNames}, check.rows = {checkRows.λ},
           check.names = {checkNames.λ},
           stringsAsFactors = {stringsAsFactors})".__call

            Return var
        End Function

        ''' <summary>
        ''' ### Data Frames
        ''' 
        ''' This function creates data frames, tightly coupled collections of variables which share many of the properties of matrices and 
        ''' of lists, used as the fundamental data structure by most of R's modeling software.
        ''' </summary>
        ''' <param name="columns"></param>
        ''' <returns></returns>
        Public Function dataframe(ParamArray columns As ArgumentReference()) As String
            Dim var As String = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- data.frame({columns.ArgumentExpression.JoinBy(", " & vbCrLf)});"
                End With
            End SyncLock

            Return var
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
            Dim strings$() = data.Select(AddressOf Scripting.ToString).ToArray
            Dim vec As String = c(strings, recursive:=False)
            Return matrix(vec, nrow, ncol, byrow, dimnames)
        End Function

        ''' <summary>
        ''' Take a sequence of vector, matrix or data-frame arguments and combine by columns or rows, respectively. 
        ''' These are generic functions with methods for other R classes.
        ''' (对datafram对象添加新的列集合)
        ''' </summary>
        ''' <param name="list">	
        ''' (generalized) vectors Or matrices. These can be given as named arguments. Other R objects may be coerced as appropriate, 
        ''' Or S4 methods may be used: see sections 'Details’ and ‘Value’. (For the "data.frame" method of cbind these can be further 
        ''' arguments to data.frame such as stringsAsFactors.)
        ''' </param>
        ''' <param name="deparselevel%">
        ''' integer controlling the construction of labels in the case of non-matrix-like arguments (for the default method):
        ''' 
        ''' + deparse.level = 0 constructs no labels; the default,
        ''' + deparse.level = 1 Or 2 constructs labels from the argument names, see the 'Value’ section below.</param>
        ''' <returns></returns>
        Public Function cbind(list As IEnumerable(Of String), Optional deparselevel% = 1) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- cbind({list.JoinBy(", ")}, deparse.level = {deparselevel})"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' Take a sequence of vector, matrix or data-frame arguments and combine by columns or rows, respectively. 
        ''' These are generic functions with methods for other R classes.
        ''' (对datafram对象拓展新的行数据)
        ''' </summary>
        ''' <param name="list">	
        ''' (generalized) vectors Or matrices. These can be given as named arguments. Other R objects may be coerced as appropriate, 
        ''' Or S4 methods may be used: see sections 'Details’ and ‘Value’. (For the "data.frame" method of cbind these can be further 
        ''' arguments to data.frame such as stringsAsFactors.)
        ''' </param>
        ''' <param name="deparselevel%">
        ''' integer controlling the construction of labels in the case of non-matrix-like arguments (for the default method):
        ''' 
        ''' + deparse.level = 0 constructs no labels; the default,
        ''' + deparse.level = 1 Or 2 constructs labels from the argument names, see the 'Value’ section below.</param>
        ''' <returns></returns>
        Public Function rbind(list As IEnumerable(Of String), Optional deparselevel% = 1) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- rbind({list.JoinBy(", ")}, deparse.level = {deparselevel})"
                End With
            End SyncLock

            Return var
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
            Throw New NotImplementedException
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
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- summary({[object]})"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' ##### Sum of Vector Elements
        ''' 
        ''' sum returns the sum of all the values present in its arguments.
        ''' </summary>
        ''' <param name="ref$">numeric or complex or logical vectors.</param>
        ''' <param name="narm">logical. Should missing values (including NaN) be removed?</param>
        ''' <returns>
        ''' The sum. If all of ... are of type integer or logical, then the sum is integer, and in that case the 
        ''' result will be NA (with a warning) if integer overflow occurs. Otherwise it is a length-one numeric 
        ''' or complex vector.
        '''
        ''' NB: the sum of an empty set is zero, by definition.
        ''' </returns>
        ''' <remarks>
        ''' This is a generic function: methods can be defined for it directly or via the Summary group generic. 
        ''' For this to work properly, the arguments ... should be unnamed, and dispatch is on the first argument.
        ''' If na.rm is FALSE an NA or NaN value in any of the arguments will cause a value of NA or NaN to be 
        ''' returned, otherwise NA and NaN values are ignored.
        ''' Logical true values are regarded as one, false values as zero. For historical reasons, NULL is accepted 
        ''' and treated as if it were integer(0).
        ''' Loss of accuracy can occur when summing values of different signs: this can even occur for sufficiently 
        ''' long integer inputs if the partial sums would cause integer overflow. Where possible extended-precision 
        ''' accumulators are used, but this is platform-dependent.
        ''' </remarks>
        Public Function sum(ref$, Optional narm As Boolean = False) As String
            SyncLock R
                With R
                    Dim var$ = RDotNetGC.Allocate
                    .call = $"{var} <- sum({ref}, na.rm = {Rbool(narm)});"
                    Return var
                End With
            End SyncLock
        End Function
    End Module
End Namespace
