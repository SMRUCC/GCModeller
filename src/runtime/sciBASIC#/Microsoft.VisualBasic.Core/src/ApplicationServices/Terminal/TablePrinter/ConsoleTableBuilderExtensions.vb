Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.TablePrinter.Flags

Namespace ApplicationServices.Terminal.TablePrinter

    <HideModuleName>
    Public Module ConsoleTableBuilderExtensions

        <Extension()>
        Public Function AddColumn(ByVal builder As ConsoleTableBuilder, ByVal columnName As String) As ConsoleTableBuilder
            builder.Column.Add(columnName)
            Return builder
        End Function

        <Extension()>
        Public Function AddColumn(ByVal builder As ConsoleTableBuilder, ByVal columnNames As List(Of String)) As ConsoleTableBuilder
            builder.Column.AddRange(columnNames)
            Return builder
        End Function

        <Extension()>
        Public Function AddColumn(ByVal builder As ConsoleTableBuilder, ParamArray columnNames As String()) As ConsoleTableBuilder
            builder.Column.AddRange(New List(Of Object)(columnNames))
            Return builder
        End Function

        <Extension()>
        Public Function WithColumn(ByVal builder As ConsoleTableBuilder, ByVal columnNames As List(Of String)) As ConsoleTableBuilder
            builder.Column = New List(Of Object)()
            builder.Column.AddRange(columnNames)
            Return builder
        End Function

        <Extension()>
        Public Function WithColumn(ByVal builder As ConsoleTableBuilder, ParamArray columnNames As String()) As ConsoleTableBuilder
            builder.Column = New List(Of Object)()
            builder.Column.AddRange(New List(Of Object)(columnNames))
            Return builder
        End Function

        <Extension()>
        Public Function AddRow(ByVal builder As ConsoleTableBuilder, ParamArray rowValues As Object()) As ConsoleTableBuilder
            If rowValues Is Nothing Then Return builder
            builder.Rows.Add(New List(Of Object)(rowValues))
            Return builder
        End Function

        <Extension()>
        Public Function WithMetadataRow(ByVal builder As ConsoleTableBuilder, ByVal position As MetaRowPositions, ByVal contentGenerator As Func(Of ConsoleTableBuilder, String)) As ConsoleTableBuilder
            Select Case position
                Case MetaRowPositions.Top

                    If builder.TopMetadataRows Is Nothing Then
                        builder.TopMetadataRows = New List(Of KeyValuePair(Of MetaRowPositions, Func(Of ConsoleTableBuilder, String)))()
                    End If

                    builder.TopMetadataRows.Add(New KeyValuePair(Of MetaRowPositions, Func(Of ConsoleTableBuilder, String))(position, contentGenerator))
                Case MetaRowPositions.Bottom

                    If builder.BottomMetadataRows Is Nothing Then
                        builder.BottomMetadataRows = New List(Of KeyValuePair(Of MetaRowPositions, Func(Of ConsoleTableBuilder, String)))()
                    End If

                    builder.BottomMetadataRows.Add(New KeyValuePair(Of MetaRowPositions, Func(Of ConsoleTableBuilder, String))(position, contentGenerator))
                Case Else
            End Select

            Return builder
        End Function

        ''' <summary>
        ''' Add title row on top of table
        ''' </summary>
        ''' <param name="builder"></param>
        ''' <param name="title"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function WithTitle(ByVal builder As ConsoleTableBuilder, ByVal title As String, ByVal Optional titleAligntment As TextAligntment = TextAligntment.Center) As ConsoleTableBuilder
            builder.TableTitle = title
            builder.TableTitleTextAlignment = titleAligntment
            Return builder
        End Function

        ''' <summary>
        ''' Add title row on top of table
        ''' </summary>
        ''' <param name="builder"></param>
        ''' <param name="title"></param>
        ''' <param name="foregroundColor">text color</param>
        ''' <returns></returns>
        <Extension()>
        Public Function WithTitle(ByVal builder As ConsoleTableBuilder, ByVal title As String, ByVal foregroundColor As ConsoleColor, ByVal Optional titleAligntment As TextAligntment = TextAligntment.Center) As ConsoleTableBuilder
            builder.TableTitle = title
            builder.TableTitleColor = New ConsoleColorNullable(foregroundColor)
            builder.TableTitleTextAlignment = titleAligntment
            Return builder
        End Function

        ''' <summary>
        ''' Add title row on top of table
        ''' </summary>
        ''' <param name="builder"></param>
        ''' <param name="title"></param>
        ''' <param name="foregroundColor">text color</param>
        ''' <param name="backgroundColor">background color</param>
        ''' <returns></returns>
        <Extension()>
        Public Function WithTitle(ByVal builder As ConsoleTableBuilder, ByVal title As String, ByVal foregroundColor As ConsoleColor, ByVal backgroundColor As ConsoleColor, ByVal Optional titleAligntment As TextAligntment = TextAligntment.Center) As ConsoleTableBuilder
            builder.TableTitle = title
            builder.TableTitleColor = New ConsoleColorNullable(foregroundColor, backgroundColor)
            builder.TableTitleTextAlignment = titleAligntment
            Return builder
        End Function

        <Extension()>
        Public Function WithPaddingLeft(ByVal builder As ConsoleTableBuilder, ByVal paddingLeft As String) As ConsoleTableBuilder
            builder.PaddingLeft = If(paddingLeft, String.Empty)
            Return builder
        End Function

        <Extension()>
        Public Function WithPaddingRight(ByVal builder As ConsoleTableBuilder, ByVal paddingRight As String) As ConsoleTableBuilder
            builder.PaddingRight = If(paddingRight, String.Empty)
            Return builder
        End Function

        <Extension()>
        Public Function WithFormatter(ByVal builder As ConsoleTableBuilder, ByVal columnIndex As Integer, ByVal formatter As Func(Of String, String)) As ConsoleTableBuilder
            If Not builder.FormatterStore.ContainsKey(columnIndex) Then
                builder.FormatterStore.Add(columnIndex, formatter)
            Else
                builder.FormatterStore(columnIndex) = formatter
            End If

            Return builder
        End Function

        <Extension()>
        Public Function WithColumnFormatter(ByVal builder As ConsoleTableBuilder, ByVal columnIndex As Integer, ByVal formatter As Func(Of String, String)) As ConsoleTableBuilder
            If Not builder.ColumnFormatterStore.ContainsKey(columnIndex) Then
                builder.ColumnFormatterStore.Add(columnIndex, formatter)
            Else
                builder.ColumnFormatterStore(columnIndex) = formatter
            End If

            Return builder
        End Function

        ''' <summary>
        ''' Text alignment definition
        ''' </summary>
        ''' <param name="builder"></param>
        ''' <param name="alignmentData"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function WithTextAlignment(ByVal builder As ConsoleTableBuilder, ByVal alignmentData As Dictionary(Of Integer, TextAligntment)) As ConsoleTableBuilder
            If alignmentData IsNot Nothing Then
                builder.TextAligmentData = alignmentData
            End If

            Return builder
        End Function

        <Extension()>
        Public Function WithHeaderTextAlignment(ByVal builder As ConsoleTableBuilder, ByVal alignmentData As Dictionary(Of Integer, TextAligntment)) As ConsoleTableBuilder
            If alignmentData IsNot Nothing Then
                builder.HeaderTextAligmentData = alignmentData
            End If

            Return builder
        End Function

        <Extension()>
        Public Function WithMinLength(ByVal builder As ConsoleTableBuilder, ByVal minLengthData As Dictionary(Of Integer, Integer)) As ConsoleTableBuilder
            If minLengthData IsNot Nothing Then
                builder.MinLengthData = minLengthData
            End If

            Return builder
        End Function

        <Extension()>
        Public Function TrimColumn(ByVal builder As ConsoleTableBuilder, ByVal Optional canTrimColumn As Boolean = True) As ConsoleTableBuilder
            builder.CanTrimColumn = canTrimColumn
            Return builder
        End Function

        <Extension()>
        Public Function AddRow(ByVal builder As ConsoleTableBuilder, ByVal row As List(Of Object)) As ConsoleTableBuilder
            If row Is Nothing Then Return builder
            builder.Rows.Add(row)
            Return builder
        End Function

        <Extension()>
        Public Function AddRow(ByVal builder As ConsoleTableBuilder, ByVal rows As List(Of List(Of Object))) As ConsoleTableBuilder
            If rows Is Nothing Then Return builder
            builder.Rows.AddRange(rows)
            Return builder
        End Function

        <Extension()>
        Public Function AddRow(ByVal builder As ConsoleTableBuilder, ByVal row As DataRow) As ConsoleTableBuilder
            If row Is Nothing Then Return builder
            builder.Rows.Add(New List(Of Object)(row.ItemArray))
            Return builder
        End Function

        <Extension()>
        Public Function WithFormat(ByVal builder As ConsoleTableBuilder, ByVal format As ConsoleTableBuilderFormat) As ConsoleTableBuilder
            ' reset CharMapPositions
            builder.CharMapPositionStore = Nothing
            builder.TableFormat = format

            Select Case builder.TableFormat
                Case ConsoleTableBuilderFormat.Default
                    builder.CharMapPositionStore = New Dictionary(Of CharMapPositions, Char) From {
                        {CharMapPositions.TopLeft, "-"c},
                        {CharMapPositions.TopCenter, "-"c},
                        {CharMapPositions.TopRight, "-"c},
                        {CharMapPositions.MiddleLeft, "-"c},
                        {CharMapPositions.MiddleCenter, "-"c},
                        {CharMapPositions.MiddleRight, "-"c},
                        {CharMapPositions.BottomLeft, "-"c},
                        {CharMapPositions.BottomCenter, "-"c},
                        {CharMapPositions.BottomRight, "-"c},
                        {CharMapPositions.BorderTop, "-"c},
                        {CharMapPositions.BorderLeft, "|"c},
                        {CharMapPositions.BorderRight, "|"c},
                        {CharMapPositions.BorderBottom, "-"c},
                        {CharMapPositions.DividerX, "-"c},
                        {CharMapPositions.DividerY, "|"c}
                    }
                Case ConsoleTableBuilderFormat.MarkDown
                    builder.CharMapPositionStore = New Dictionary(Of CharMapPositions, Char) From {
                        {CharMapPositions.DividerY, "|"c},
                        {CharMapPositions.BorderLeft, "|"c},
                        {CharMapPositions.BorderRight, "|"c}
                    }
                    builder.HeaderCharMapPositionStore = New Dictionary(Of HeaderCharMapPositions, Char) From {
                        {HeaderCharMapPositions.BorderBottom, "-"c},
                        {HeaderCharMapPositions.BottomLeft, "|"c},
                        {HeaderCharMapPositions.BottomCenter, "|"c},
                        {HeaderCharMapPositions.BottomRight, "|"c},
                        {HeaderCharMapPositions.BorderLeft, "|"c},
                        {HeaderCharMapPositions.BorderRight, "|"c},
                        {HeaderCharMapPositions.Divider, "|"c}
                    }
                Case ConsoleTableBuilderFormat.Alternative
                    builder.CharMapPositionStore = New Dictionary(Of CharMapPositions, Char) From {
                        {CharMapPositions.TopLeft, "+"c},
                        {CharMapPositions.TopCenter, "+"c},
                        {CharMapPositions.TopRight, "+"c},
                        {CharMapPositions.MiddleLeft, "+"c},
                        {CharMapPositions.MiddleCenter, "+"c},
                        {CharMapPositions.MiddleRight, "+"c},
                        {CharMapPositions.BottomLeft, "+"c},
                        {CharMapPositions.BottomCenter, "+"c},
                        {CharMapPositions.BottomRight, "+"c},
                        {CharMapPositions.BorderTop, "-"c},
                        {CharMapPositions.BorderRight, "|"c},
                        {CharMapPositions.BorderBottom, "-"c},
                        {CharMapPositions.BorderLeft, "|"c},
                        {CharMapPositions.DividerX, "-"c},
                        {CharMapPositions.DividerY, "|"c}
                    }
                Case ConsoleTableBuilderFormat.Minimal
                    builder.CharMapPositionStore = New Dictionary(Of CharMapPositions, Char) From {
                    }
                    builder.HeaderCharMapPositionStore = New Dictionary(Of HeaderCharMapPositions, Char) From {
                        {HeaderCharMapPositions.BorderBottom, "-"c}
                    }
                    builder.PaddingLeft = String.Empty
                    builder.PaddingRight = " "
                Case Else
            End Select

            Return builder
        End Function

        <Extension()>
        Public Function WithCharMapDefinition(ByVal builder As ConsoleTableBuilder) As ConsoleTableBuilder
            Return builder.WithCharMapDefinition(New Dictionary(Of CharMapPositions, Char) From {
            })
        End Function

        <Extension()>
        Public Function WithCharMapDefinition(ByVal builder As ConsoleTableBuilder, ByVal charMapPositions As Dictionary(Of CharMapPositions, Char)) As ConsoleTableBuilder
            builder.CharMapPositionStore = charMapPositions
            Return builder
        End Function

        <Extension()>
        Public Function WithCharMapDefinition(ByVal builder As ConsoleTableBuilder, ByVal charMapPositions As Dictionary(Of CharMapPositions, Char), ByVal Optional headerCharMapPositions As Dictionary(Of HeaderCharMapPositions, Char) = Nothing) As ConsoleTableBuilder
            builder.CharMapPositionStore = charMapPositions
            builder.HeaderCharMapPositionStore = headerCharMapPositions
            Return builder
        End Function

        <Extension()>
        Public Function WithHeaderCharMapDefinition(ByVal builder As ConsoleTableBuilder, ByVal Optional headerCharMapPositions As Dictionary(Of HeaderCharMapPositions, Char) = Nothing) As ConsoleTableBuilder
            builder.HeaderCharMapPositionStore = headerCharMapPositions
            Return builder
        End Function

        <Extension()>
        Public Function Export(ByVal builder As ConsoleTableBuilder) As StringBuilder
            Dim numberOfColumns = 0

            If builder.Rows.Any() Then
                numberOfColumns = builder.Rows.Max(Function(x) x.Count)
            Else

                If builder.Column IsNot Nothing Then
                    numberOfColumns = builder.Column.Count()
                End If
            End If

            If numberOfColumns = 0 Then
                Return New StringBuilder()
            End If

            If builder.Column Is Nothing Then
                numberOfColumns = 0
            Else

                If numberOfColumns < builder.Column.Count Then
                    numberOfColumns = builder.Column.Count
                End If
            End If

            For i = 0 To 1 - 1

                If builder.Column IsNot Nothing AndAlso builder.Column.Count < numberOfColumns Then
                    Dim missCount = numberOfColumns - builder.Column.Count

                    For j = 0 To missCount - 1
                        builder.Column.Add(Nothing)
                    Next
                End If
            Next

            For i = 0 To builder.Rows.Count - 1

                If builder.Rows(i).Count < numberOfColumns Then
                    Dim missCount = numberOfColumns - builder.Rows(i).Count

                    For j = 0 To missCount - 1
                        builder.Rows(i).Add(Nothing)
                    Next
                End If
            Next

            Return CreateTableForCustomFormat(builder)
        End Function

        <Extension()>
        Public Sub ExportAndWrite(ByVal builder As ConsoleTableBuilder, ByVal Optional alignment As TableAligntment = TableAligntment.Left)
            Dim strBuilder = builder.Export()
            Dim lines = strBuilder.ToString().Split(Microsoft.VisualBasic.Strings.ChrW(10))
            Dim linesCount = lines.Count()
            Dim pad As Integer

            For i = 0 To linesCount - 1
                Dim row = String.Empty

                Select Case alignment
                    Case TableAligntment.Left
                        row = lines(i)
                    Case TableAligntment.Center
                        pad = Console.WindowWidth / 2 + lines(i).RealLength(True) / 2 - (lines(i).RealLength(True) - lines(i).Length)
                        row = String.Format("{0," & pad & "}", lines(i))
                    Case TableAligntment.Right
                        row = New String(" "c, Console.WindowWidth - lines(i).RealLength(True)) & lines(i)
                End Select

                If i = 0 AndAlso Not String.IsNullOrEmpty(builder.TableTitle) AndAlso builder.TableTitle.Trim().Length <> 0 AndAlso Not builder.TableTitleColor.IsForegroundColorNull AndAlso builder.TitlePositionStartAt > 0 AndAlso builder.TitlePositionLength > 0 Then
                    Dim newTitlePositionStartAt = builder.TitlePositionStartAt + (row.Length - lines(i).Length)
                    Console.Write(row.Substring(0, newTitlePositionStartAt))
                    Console.ForegroundColor = builder.TableTitleColor.ForegroundColor

                    If Not builder.TableTitleColor.IsBackgroundColorNull Then
                        Console.BackgroundColor = builder.TableTitleColor.BackgroundColor
                    End If

                    Console.Write(row.Substring(newTitlePositionStartAt, builder.TitlePositionLength))
                    Console.ResetColor()
                    Console.Write(row.Substring(newTitlePositionStartAt + builder.TitlePositionLength, row.Length - (newTitlePositionStartAt + builder.TitlePositionLength)))
                    Console.Write(Microsoft.VisualBasic.Strings.ChrW(10))
                Else

                    If i = linesCount - 2 Then
                        If row.EndsWith(Microsoft.VisualBasic.Strings.ChrW(13).ToString()) Then
                            Console.Write(row.Substring(0, row.Length - 1))
                        Else
                            Console.Write(row)
                        End If
                    Else

                        If i = linesCount - 1 Then ' is last line
                            Console.Write(row)
                        Else
                            Console.WriteLine(row)
                        End If
                    End If
                End If
            Next
        End Sub

        <Extension()>
        Public Sub ExportAndWriteLine(ByVal builder As ConsoleTableBuilder, ByVal Optional alignment As TableAligntment = TableAligntment.Left)
            builder.ExportAndWrite(alignment)
            Console.Write(Microsoft.VisualBasic.Strings.ChrW(10))
        End Sub

        Private Function CreateTableForCustomFormat(ByVal builder As ConsoleTableBuilder) As StringBuilder
            If builder.CharMapPositionStore Is Nothing Then
                builder.WithFormat(ConsoleTableBuilderFormat.Default)
            End If

            builder.PopulateFormattedColumnsRows()
            Dim columnLengths = builder.GetCadidateColumnLengths()
            Dim columnNoUtf8CharasLengths = builder.GetCadidateColumnLengths(False)
            builder.CenterRowContent(columnLengths)
            Dim filledMap = FillCharMap(builder.CharMapPositionStore)
            Dim filledHeaderMap = FillHeaderCharMap(builder.HeaderCharMapPositionStore)
            Dim strBuilder = New StringBuilder()
            Dim topMetadataStringBuilder = BuildMetaRowsFormat(builder, MetaRowPositions.Top)

            For i = 0 To topMetadataStringBuilder.Count - 1
                strBuilder.AppendLine(topMetadataStringBuilder(i))
            Next

            Dim tableTopLine = builder.CreateTableTopLine(columnLengths, filledMap)
            Dim tableRowContentFormat = builder.CreateTableContentLineFormat(columnLengths, filledMap)
            Dim tableMiddleLine = builder.CreateTableMiddleLine(columnLengths, filledMap)
            Dim tableBottomLine = builder.CreateTableBottomLine(columnLengths, filledMap)
            Dim headerTopLine = String.Empty
            Dim headerRowContentFormat = String.Empty
            Dim headerBottomLine = String.Empty

            If filledHeaderMap IsNot Nothing Then
                headerTopLine = builder.CreateHeaderTopLine(columnLengths, filledMap, filledHeaderMap)
                headerRowContentFormat = builder.CreateHeaderContentLineFormat(columnLengths, filledMap, filledHeaderMap)
                headerBottomLine = builder.CreateHeaderBottomLine(columnLengths, filledMap, filledHeaderMap)
            End If

            ' find the longest formatted line
            'var maxRowLength = Math.Max(0, builder.Rows.Any() ? builder.Rows.Max(row => string.Format(tableRowContentFormat, row.ToArray()).Length) : 0);

            Dim hasHeader = builder.FormattedColumns IsNot Nothing AndAlso builder.FormattedColumns.Any() AndAlso builder.FormattedColumns.Max(Function(x) If(x, String.Empty).ToString().Length) > 0

            ' header
            If hasHeader Then
                If Not Equals(headerTopLine, Nothing) AndAlso headerTopLine.Trim().Length > 0 Then
                    strBuilder.AppendLine(headerTopLine)
                Else

                    If Not Equals(tableTopLine, Nothing) AndAlso tableTopLine.Trim().Length > 0 Then
                        strBuilder.AppendLine(tableTopLine)
                    End If
                End If

                Dim headerSlices = builder.FormattedColumns.ToArray()
                Dim formattedHeaderSlice = builder.CenterColumnContent(headerSlices, columnLengths)

                'var formattedHeaderSlice = Enumerable.Range(0, headerSlices.Length).Select(idx => builder.ColumnFormatterStore.ContainsKey(idx) ? builder.ColumnFormatterStore[idx](headerSlices[idx] == null ? string.Empty : headerSlices[idx].ToString()) : headerSlices[idx] == null ? string.Empty : headerSlices[idx].ToString()).ToArray();
                'formattedHeaderSlice = builder.CenterColumnContent(headerSlices, columnLengths);

                If Not Equals(headerRowContentFormat, Nothing) AndAlso headerRowContentFormat.Trim().Length > 0 Then
                    strBuilder.AppendLine(String.Format(headerRowContentFormat, formattedHeaderSlice))
                Else
                    strBuilder.AppendLine(String.Format(tableRowContentFormat, formattedHeaderSlice))
                End If
            End If
            'else
            '{
            '    if (beginTableFormat.Length > 0) strBuilder.AppendLine(beginTableFormat);
            '    strBuilder.AppendLine(string.Format(rowContentTableFormat, builder.FormattedColumns.ToArray()));
            '}

            ' add each row

            'var results = builder.Rows.Select(row => {
            '    var rowSlices = row.ToArray();
            '    return string.Format(tableRowContentFormat, Enumerable.Range(0, rowSlices.Length).Select(idx => builder.FormatterStore.ContainsKey(idx) ? builder.FormatterStore[idx](rowSlices[idx] == null ? string.Empty : rowSlices[idx].ToString()) : rowSlices[idx] == null ? string.Empty : rowSlices[idx].ToString()).ToArray());
            '}).ToList();

            Dim results = builder.FormattedRows.[Select](Function(row)
                                                             Dim rowFormate = builder.CreateRawLineFormat(columnLengths, filledMap, row.ToArray())
                                                             Return String.Format(rowFormate, row.ToArray())
                                                         End Function).ToList()
            Dim isFirstRow = True

            For Each row In results

                If isFirstRow Then
                    If hasHeader Then
                        If (String.IsNullOrEmpty(headerBottomLine) OrElse headerBottomLine.Length = 0) AndAlso tableMiddleLine.Length > 0 Then
                            strBuilder.AppendLine(tableMiddleLine)
                        Else

                            If headerBottomLine.Length > 0 Then
                                strBuilder.AppendLine(headerBottomLine)
                            End If
                        End If
                    Else

                        If tableTopLine.Length > 0 Then
                            strBuilder.AppendLine(tableTopLine)
                        End If
                    End If

                    isFirstRow = False
                Else

                    If tableMiddleLine.Length > 0 Then
                        strBuilder.AppendLine(tableMiddleLine)
                    End If
                End If

                strBuilder.AppendLine(row)
            Next

            If results.Any() Then
                If tableBottomLine.Length > 0 Then
                    strBuilder.AppendLine(tableBottomLine)
                End If
            Else

                If (String.IsNullOrEmpty(headerBottomLine) OrElse headerBottomLine.Length = 0) AndAlso tableBottomLine.Length > 0 Then
                    strBuilder.AppendLine(tableBottomLine)
                Else

                    If headerBottomLine.Length > 0 Then
                        strBuilder.AppendLine(headerBottomLine)
                    End If
                End If
            End If

            Dim bottomMetadataStringBuilder = BuildMetaRowsFormat(builder, MetaRowPositions.Bottom)

            For i = 0 To bottomMetadataStringBuilder.Count - 1
                strBuilder.AppendLine(bottomMetadataStringBuilder(i))
            Next

            Return strBuilder
        End Function

        'private static StringBuilder CreateTableForDefaultFormat(ConsoleTableBuilder builder)
        '{
        '    var strBuilder = new StringBuilder();
        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Top);

        '    // create the string format with padding
        '    var format = builder.Format('|');

        '    if (format == string.Empty)
        '    {
        '        return strBuilder;
        '    }

        '    // find the longest formatted line
        '    var maxRowLength = Math.Max(0, builder.Rows.Any() ? builder.Rows.Max(row => string.Format(format, row.ToArray()).Length) : 0);

        '    // add each row
        '    var results = builder.Rows.Select(row => string.Format(format, row.ToArray())).ToList();

        '    // create the divider
        '    var divider = new string('-', maxRowLength);

        '    // header
        '    if (builder.Column != null && builder.Column.Any() && builder.Column.Max(x => (x ?? string.Empty).ToString().Length) > 0)
        '    {
        '        strBuilder.AppendLine(divider);
        '        strBuilder.AppendLine(string.Format(format, builder.Column.ToArray()));
        '    }

        '    foreach (var row in results)
        '    {
        '        strBuilder.AppendLine(divider);
        '        strBuilder.AppendLine(row);
        '    }

        '    strBuilder.AppendLine(divider);

        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Bottom);
        '    return strBuilder;
        '}

        'private static StringBuilder CreateTableForMinimalFormat(ConsoleTableBuilder builder)
        '{
        '    var strBuilder = new StringBuilder();
        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Top);

        '    // create the string format with padding
        '    var format = builder.Format('\0').Trim();

        '    if (format == string.Empty)
        '    {
        '        return strBuilder;
        '    }

        '    var skipFirstRow = false;
        '    var columnHeaders = string.Empty;

        '    if (builder.Column != null && builder.Column.Any() && builder.Column.Max(x => (x ?? string.Empty).ToString().Length) > 0)
        '    {
        '        skipFirstRow = false;
        '        columnHeaders = string.Format(format, builder.Column.ToArray());
        '    }
        '    else
        '    {
        '        skipFirstRow = true;
        '        columnHeaders = string.Format(format, builder.Rows.First().ToArray());
        '    }

        '    // create the divider
        '    var divider = Regex.Replace(columnHeaders, @"[^|]", '-'.ToString());

        '    strBuilder.AppendLine(columnHeaders);
        '    strBuilder.AppendLine(divider);

        '    // add each row
        '    var results = builder.Rows.Skip(skipFirstRow ? 1 : 0).Select(row => string.Format(format, row.ToArray())).ToList();
        '    results.ForEach(row => strBuilder.AppendLine(row));

        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Bottom);

        '    return strBuilder;
        '}

        'private static StringBuilder CreateTableForMarkdownFormat(ConsoleTableBuilder builder)
        '{
        '    var strBuilder = new StringBuilder();
        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Top);

        '    // create the string format with padding
        '    var format = builder.Format('|');

        '    if (format == string.Empty)
        '    {
        '        return strBuilder;
        '    }

        '    var skipFirstRow = false;
        '    var columnHeaders = string.Empty;

        '    if (builder.Column != null && builder.Column.Any() && builder.Column.Max(x => (x ?? string.Empty).ToString().Length) > 0)
        '    {
        '        skipFirstRow = false;
        '        columnHeaders = string.Format(format, builder.Column.ToArray());
        '    }
        '    else
        '    {
        '        skipFirstRow = true;
        '        columnHeaders = string.Format(format, builder.Rows.First().ToArray());
        '    }

        '    // create the divider
        '    var divider = Regex.Replace(columnHeaders, @"[^|]", '-'.ToString());

        '    strBuilder.AppendLine(columnHeaders);
        '    strBuilder.AppendLine(divider);

        '    // add each row
        '    var results = builder.Rows.Skip(skipFirstRow ? 1 : 0).Select(row => string.Format(format, row.ToArray())).ToList();
        '    results.ForEach(row => strBuilder.AppendLine(row));

        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Bottom);

        '    return strBuilder;
        '}

        'private static StringBuilder CreateTableForAlternativeFormat(ConsoleTableBuilder builder)
        '{
        '    var strBuilder = new StringBuilder();
        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Top);

        '    // create the string format with padding
        '    var format = builder.Format('|');

        '    if (format == string.Empty)
        '    {
        '        return strBuilder;
        '    }

        '    var skipFirstRow = false;
        '    var columnHeaders = string.Empty;

        '    if (builder.Column != null && builder.Column.Any() && builder.Column.Max(x => (x ?? string.Empty).ToString().Length) > 0)
        '    {
        '        skipFirstRow = false;
        '        columnHeaders = string.Format(format, builder.Column.ToArray());
        '    }
        '    else
        '    {
        '        skipFirstRow = true;
        '        columnHeaders = string.Format(format, builder.Rows.First().ToArray());
        '    }

        '    // create the divider
        '    var divider = Regex.Replace(columnHeaders, @"[^|]", '-'.ToString());
        '    var dividerPlus = divider.Replace("|", "+");

        '    strBuilder.AppendLine(dividerPlus);
        '    strBuilder.AppendLine(columnHeaders);

        '    // add each row
        '    var results = builder.Rows.Skip(skipFirstRow ? 1 : 0).Select(row => string.Format(format, row.ToArray())).ToList();

        '    foreach (var row in results)
        '    {
        '        strBuilder.AppendLine(dividerPlus);
        '        strBuilder.AppendLine(row);
        '    }
        '    strBuilder.AppendLine(dividerPlus);

        '    BuildMetaRowsFormat(builder, strBuilder, MetaRowPositions.Bottom);
        '    return strBuilder;
        '}

        Private Function BuildMetaRowsFormat(ByVal builder As ConsoleTableBuilder, ByVal position As MetaRowPositions) As List(Of String)
            Dim result = New List(Of String)()

            Select Case position
                Case MetaRowPositions.Top

                    If builder.TopMetadataRows.Any() Then
                        For Each item In builder.TopMetadataRows

                            If item.Value IsNot Nothing Then
                                result.Add(item.Value.Invoke(builder))
                            End If
                        Next
                    End If

                Case MetaRowPositions.Bottom

                    If builder.BottomMetadataRows.Any() Then
                        For Each item In builder.BottomMetadataRows

                            If item.Value IsNot Nothing Then
                                result.Add(item.Value.Invoke(builder))
                            End If
                        Next
                    End If

                Case Else
            End Select

            Return result
        End Function

        Private Function FillCharMap(ByVal definition As Dictionary(Of CharMapPositions, Char)) As Dictionary(Of CharMapPositions, Char)
            If definition Is Nothing Then
                Return New Dictionary(Of CharMapPositions, Char)()
            End If

            Dim filledMap = definition

            For Each c In CType([Enum].GetValues(GetType(CharMapPositions)), CharMapPositions())

                If Not filledMap.ContainsKey(c) Then
                    filledMap.Add(c, Microsoft.VisualBasic.Strings.ChrW(0))
                End If
            Next

            Return filledMap
        End Function

        Private Function FillHeaderCharMap(ByVal definition As Dictionary(Of HeaderCharMapPositions, Char)) As Dictionary(Of HeaderCharMapPositions, Char)
            If definition Is Nothing Then
                Return Nothing
            End If

            Dim filledMap = definition

            For Each c In CType([Enum].GetValues(GetType(HeaderCharMapPositions)), HeaderCharMapPositions())

                If Not filledMap.ContainsKey(c) Then
                    filledMap.Add(c, Microsoft.VisualBasic.Strings.ChrW(0))
                End If
            Next

            Return filledMap
        End Function

        <Extension()>
        Public Function RealLength(ByVal value As String, ByVal withUtf8Characters As Boolean) As Integer
            If String.IsNullOrEmpty(value) Then Return 0
            If Not withUtf8Characters Then Return value.Length
            Dim i = 0 'count

            For Each newChar In value.Select(AddressOf AscW)
                If newChar >= &H4E00 AndAlso newChar <= &H9FBB Then
                    'utf-8 characters
                    i += 2
                Else
                    i += 1
                End If
            Next

            Return i
        End Function
    End Module
End Namespace
