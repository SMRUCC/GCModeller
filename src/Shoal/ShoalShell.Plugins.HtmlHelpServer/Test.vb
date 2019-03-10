Imports Oracle.LinuxCompatibility.MySQL

Module Test

    Sub Main()
        Dim linqTest As Wiki.MySQL.lib_wikiarchive() = (
            From x As Wiki.MySQL.lib_wikiarchive
            In "https://127.0.0.1:3306/client?user=oWZwI0JCsf4Z6SDzIKjVXg==%password=f5DGNogWz0se2ScJ2rsu5Q==%database=PX9nLOBmuJkV700zowzCeQ==".AsDBI(Of Wiki.MySQL.lib_wikiarchive) <= "SELECT * FROM wiki.lib_wikiarchive;"
            Select x).ToArray
    End Sub
End Module
