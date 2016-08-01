Namespace API

    Public Module base

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
            Dim Rscript As String = $"require({package},lib.loc={libloc},quietly={quietly.R},warn.conflicts={warnConflicts.R},character.only={characterOnly.R})"
            Return RServer.Evaluate(Rscript).AsLogical.First
        End Function
    End Module
End Namespace