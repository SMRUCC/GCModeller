#Region "Microsoft.VisualBasic::9872d4371e3aa1318604e8ceaf5cf7ab, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\API\base.vb"

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

Namespace API

    Public Module base

        ''' <summary>
        ''' Combine Values into a Vector or List
        ''' 
        ''' This is a generic function which combines its arguments.
        ''' The Default method combines its arguments To form a vector. All arguments are coerced To a common type which Is the type Of the returned value, And all attributes except names are removed.
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
            Call $"{out} <- c({list.JoinBy(", ")}, recursive = {recursive.λ})".丶
            Return out
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
            Return c(list, False)
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
                           verbose = {verbose})".丶
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
                        character.only = {characterOnly.λ})".丶.AsBoolean
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
           stringsAsFactors = {stringsAsFactors})".丶

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

            Dim x As String = App.NextTempName

            If nrow = -1 Then
                Call $"{x} <- matrix({data}, ncol={ncol}, byrow={byrow.λ}, dimnames={dimnames})".丶
            ElseIf ncol = -1 Then
                Call $"{x} <- matrix({data}, nrow={nrow}, byrow={byrow.λ}, dimnames={dimnames})".丶
            Else
                Call $"{x} <- matrix({data}, nrow={nrow}, ncol={ncol}, byrow={byrow.λ}, dimnames={dimnames})".丶
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
            Dim vec As String = c(data.ToArray)
            Return matrix(vec, nrow, ncol, byrow, dimnames)
        End Function
    End Module
End Namespace
