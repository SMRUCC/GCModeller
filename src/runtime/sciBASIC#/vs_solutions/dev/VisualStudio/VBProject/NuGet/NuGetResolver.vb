Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Xml.Linq

Namespace VBProj.NuGet

    ''' <summary>
    ''' nuget 依赖解析器: 读取包内 <c>.nuspec</c>, 递归展开传递依赖,
    ''' 完成目标框架匹配、版本冲突消解与资产(托管 dll / 原生库目录)选择。
    ''' </summary>
    ''' <remarks>
    ''' 解析策略:
    ''' <list type="number">
    ''' <item><b>依赖展开</b>: 以广度优先方式展开依赖图, 每个包只保留一个最终版本;</item>
    ''' <item><b>版本冲突</b>: 与 NuGet 一致地"取较高版本" —— 在同一 id 上累积全部版本约束,
    ''' 优先选择同时满足最多约束的最高版本, 因此版本选择单调不降, 展开过程必然收敛;</item>
    ''' <item><b>框架匹配</b>: 依赖组与资产目录均按 <see cref="NuGetFramework"/> 的等级模型
    ''' 选择"不高于目标框架的最高等级"者;</item>
    ''' <item><b>资产</b>: 只收集 <c>lib/</c> 与当前平台 <c>runtimes/&lt;rid&gt;/lib</c> 下的托管 dll
    ''' (排除附属资源 <c>*.resources.dll</c>), <c>ref/</c> 为纯引用程序集(无 IL)因此不用于运行期。</item>
    ''' </list>
    ''' </remarks>
    Public Class NuGetResolver

        ''' <summary>依赖展开的最大迭代次数(防御性上限, 正常包图远小于此)</summary>
        Private Const MaxIterations As Integer = 4096

        ''' <summary>包获取客户端</summary>
        Public Property Client As NuGetClient

        ''' <summary>目标框架 moniker</summary>
        Public Property TargetFramework As String

        ''' <summary>是否允许选择 prerelease 版本</summary>
        Public Property AllowPrerelease As Boolean

        Public Sub New(Optional targetFramework As String = "net10.0",
                       Optional allowPrerelease As Boolean = False,
                       Optional client As NuGetClient = Nothing)

            Me.TargetFramework = If(String.IsNullOrWhiteSpace(targetFramework), "net10.0", targetFramework.Trim())
            Me.AllowPrerelease = allowPrerelease
            Me.Client = If(client, New NuGetClient())
        End Sub

        ''' <summary>
        ''' 解析一个 nuget 包及其全部传递依赖。
        ''' </summary>
        ''' <param name="packageId">包 id</param>
        ''' <param name="versionConstraint">版本或版本范围; 空/Nothing 表示取最新稳定版</param>
        ''' <param name="targetFramework">目标框架 moniker(如 net10.0)</param>
        ''' <param name="allowPrerelease">是否允许 prerelease 版本</param>
        Public Shared Function Resolve(packageId As String,
                                       versionConstraint As String,
                                       Optional targetFramework As String = "net10.0",
                                       Optional allowPrerelease As Boolean = False) As NuGetResolveResult

            Return New NuGetResolver(targetFramework, allowPrerelease).ResolvePackages(packageId, versionConstraint)
        End Function

        ''' <summary>
        ''' 解析入口: 返回根包与全部传递依赖(每个 id 只保留最终选定的版本)。
        ''' </summary>
        ''' <remarks>
        ''' 参数刻意命名为 <c>versionConstraint</c> 而非 <c>versionRange</c>:
        ''' VB 是大小写不敏感的, 若命名为 <c>versionRange</c> 会屏蔽同名的
        ''' <see cref="VersionRange"/> 类型, 导致 <c>VersionRange.Parse</c> 解析失败。
        ''' </remarks>
        Public Function ResolvePackages(packageId As String, versionConstraint As String) As NuGetResolveResult
            If String.IsNullOrWhiteSpace(packageId) Then
                Throw New NuGetException("nuget 包 id 不能为空")
            End If

            Dim requirements As New Dictionary(Of String, List(Of VersionRange))(StringComparer.OrdinalIgnoreCase)
            Dim displayIds As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim resolved As New Dictionary(Of String, NuGetPackage)(StringComparer.OrdinalIgnoreCase)
            Dim pending As New Queue(Of KeyValuePair(Of String, VersionRange))

            Call EnqueueRequirement(packageId, VersionRange.Parse(versionConstraint), requirements, displayIds, pending)

            Dim rootKey As String = packageId.Trim().ToLowerInvariant()
            Dim iteration As Integer = 0

            While pending.Count > 0
                iteration += 1

                If iteration > MaxIterations Then
                    Throw New NuGetException($"nuget 依赖解析超过最大迭代次数({MaxIterations}), 可能存在异常的依赖关系")
                End If

                Dim requirement As KeyValuePair(Of String, VersionRange) = pending.Dequeue()
                Dim key As String = requirement.Key.ToLowerInvariant()
                Dim existing As NuGetPackage = Nothing
                Dim needResolve As Boolean = True

                If resolved.TryGetValue(key, existing) Then
                    If requirement.Value.Satisfies(existing.Version) Then
                        needResolve = False
                    End If
                End If

                If Not needResolve Then
                    Continue While
                End If

                Dim selected As NuGetVersion = SelectVersion(displayIds(key), requirements(key))

                If existing IsNot Nothing AndAlso existing.Version.CompareTo(selected) = 0 Then
                    Continue While
                End If

                Dim package As NuGetPackage = LoadPackage(displayIds(key), selected)
                resolved(key) = package

                For Each dependency As NuGetDependency In package.Dependencies
                    Call EnqueueRequirement(dependency.Id, dependency.Range, requirements, displayIds, pending)
                Next
            End While

            Dim result As New NuGetResolveResult()
            Dim root As NuGetPackage = Nothing

            If resolved.TryGetValue(rootKey, root) Then
                root.IsRoot = True
                Call result.Packages.Add(root)
            Else
                Throw New NuGetException($"nuget 包 '{packageId}' 解析失败: 未得到任何可用版本")
            End If

            For Each package As NuGetPackage In resolved.Values
                If Not package Is root Then
                    Call result.Packages.Add(package)
                End If
            Next

            result.Root = root

            Return result
        End Function

        Private Shared Sub EnqueueRequirement(id As String,
                                              range As VersionRange,
                                              requirements As Dictionary(Of String, List(Of VersionRange)),
                                              displayIds As Dictionary(Of String, String),
                                              pending As Queue(Of KeyValuePair(Of String, VersionRange)))

            If String.IsNullOrWhiteSpace(id) Then
                Return
            End If

            Dim name As String = id.Trim()
            Dim key As String = name.ToLowerInvariant()
            Dim list As List(Of VersionRange) = Nothing

            If Not requirements.TryGetValue(key, list) Then
                list = New List(Of VersionRange)
                requirements(key) = list
                displayIds(key) = name
            End If

            Dim constraint As VersionRange = If(range, VersionRange.All)

            Call list.Add(constraint)
            Call pending.Enqueue(New KeyValuePair(Of String, VersionRange)(name, constraint))
        End Sub

        ''' <summary>
        ''' 在全部已有约束下选择版本: 优先"满足约束最多"的版本, 同级取最高版本。
        ''' </summary>
        Private Function SelectVersion(packageId As String, constraints As List(Of VersionRange)) As NuGetVersion
            Dim versions As NuGetVersion() = Client.GetVersions(packageId)
            Dim best As NuGetVersion = Nothing
            Dim bestScore As Integer = -1

            For Each version As NuGetVersion In versions
                If version.IsPrerelease AndAlso Not AllowPrerelease Then
                    Continue For
                End If

                ' 注意: 不能使用 List.Count(predicate) —— VB 会把 List(Of T).Count 属性
                ' 视作实例成员而屏蔽 System.Linq 的 Count 扩展方法, 因此改用 Where(...).Count()
                Dim score As Integer = constraints.Where(Function(c) c.Satisfies(version)).Count()

                If score = 0 Then
                    Continue For
                End If

                If score > bestScore Then
                    bestScore = score
                    best = version
                ElseIf score = bestScore AndAlso best IsNot Nothing AndAlso version.CompareTo(best) > 0 Then
                    ' versions 已按降序排列, 这里只是防御性写法
                    best = version
                End If
            Next

            If best Is Nothing Then
                Throw New NuGetException(
                    $"nuget 包 '{packageId}' 不存在满足依赖约束的版本" & vbCrLf &
                    $"约束条件: {String.Join("; ", constraints)}" & vbCrLf &
                    $"可用版本: {String.Join(", ", versions.Take(10).Select(Function(v) v.ToString()))}")
            End If

            Return best
        End Function

        ''' <summary>把包取到本地缓存, 读取 nuspec, 解析依赖与资产</summary>
        Private Function LoadPackage(packageId As String, version As NuGetVersion) As NuGetPackage
            Dim folder As String = Client.Install(packageId, version)
            Dim nuspec As String = NuGetClient.FindNuspec(folder)

            If nuspec Is Nothing Then
                Throw New NuGetException($"nuget 包 '{packageId}@{version}' 中找不到 .nuspec 描述文件: {folder}")
            End If

            Dim descriptor As NuspecDescriptor = ParseNuspec(nuspec, packageId, version)
            Dim package As New NuGetPackage With {
                .Id = descriptor.Id,
                .Version = descriptor.Version,
                .PackageFolder = folder,
                .NuspecFile = nuspec,
                .TargetFramework = TargetFramework
            }

            Dim group As NuGetDependencyGroup = SelectDependencyGroup(descriptor.DependencyGroups)

            If group IsNot Nothing Then
                For Each dependency As NuGetDependency In group.Dependencies
                    Call package.Dependencies.Add(New NuGetDependency With {
                        .Id = dependency.Id,
                        .Range = dependency.Range,
                        .TargetFramework = group.TargetFramework
                    })
                Next
            End If

            Call FillAssets(package)

            Return package
        End Function

        ''' <summary>选择与目标框架最匹配的依赖组</summary>
        Private Function SelectDependencyGroup(groups As List(Of NuGetDependencyGroup)) As NuGetDependencyGroup
            If groups Is Nothing OrElse groups.Count = 0 Then
                Return Nothing
            End If

            Dim best As String = NuGetFramework.GetBest(groups.Select(Function(g) g.TargetFramework), TargetFramework)

            If best Is Nothing Then
                Return Nothing
            End If

            Return groups.First(Function(g) String.Equals(g.TargetFramework, best, StringComparison.OrdinalIgnoreCase))
        End Function

        ' ==================================================================
        ' 资产选择
        ' ==================================================================

        Private Sub FillAssets(package As NuGetPackage)
            Dim folder As String = package.PackageFolder

            ' 1) 通用资产: lib/<best-tfm>
            Dim libFolder As String = FindBestFrameworkFolder(Path.Combine(folder, "lib"))

            If libFolder IsNot Nothing Then
                For Each dll As String In EnumerateAssemblies(libFolder)
                    Call AddAssembly(package, dll)
                Next
            End If

            ' 2) 当前平台资产覆盖: runtimes/<rid>/lib/<best-tfm>, 同名程序集以 RID 版本为准
            Dim rid As String = NuGetFramework.GetCurrentRuntimeIdentifier()
            Dim osName As String = rid.Split("-"c)(0)

            For Each runtime As String In {rid, osName, "any"}
                Dim runtimeLib As String = FindBestFrameworkFolder(Path.Combine(folder, "runtimes", runtime, "lib"))

                If runtimeLib IsNot Nothing Then
                    For Each dll As String In EnumerateAssemblies(runtimeLib)
                        Call AddAssembly(package, dll)
                    Next
                End If

                Dim native As String = Path.Combine(folder, "runtimes", runtime, "native")

                If Directory.Exists(native) AndAlso Directory.GetFiles(native).Length > 0 Then
                    Call package.NativeAssets.Add(native)
                End If
            Next
        End Sub

        ''' <summary>在给定根目录下挑选与目标框架最匹配的子目录</summary>
        Private Function FindBestFrameworkFolder(root As String) As String
            If Not Directory.Exists(root) Then
                Return Nothing
            End If

            Dim map As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim names As New List(Of String)

            For Each dir As String In Directory.GetDirectories(root)
                Dim name As String = Path.GetFileName(dir)

                If name.StartsWith(".") Then
                    Continue For
                End If

                If Not map.ContainsKey(name) Then
                    map(name) = dir
                    Call names.Add(name)
                End If
            Next

            If names.Count = 0 Then
                Return Nothing
            End If

            Dim best As String = NuGetFramework.GetBest(names, TargetFramework)

            If best Is Nothing Then
                Return Nothing
            End If

            Return map(best)
        End Function

        Private Shared Sub AddAssembly(package As NuGetPackage, dll As String)
            Dim name As String = Path.GetFileName(dll)
            Dim index As Integer = package.Assemblies.FindIndex(
                Function(existing) String.Equals(Path.GetFileName(existing), name, StringComparison.OrdinalIgnoreCase))

            If index >= 0 Then
                ' 平台特定资产覆盖通用资产
                package.Assemblies(index) = dll
            Else
                Call package.Assemblies.Add(dll)
            End If
        End Sub

        Private Shared Function EnumerateAssemblies(folder As String) As IEnumerable(Of String)
            Return Directory.GetFiles(folder, "*.dll", SearchOption.TopDirectoryOnly) _
                .Where(Function(file)
                           Dim name As String = Path.GetFileName(file)

                           Return Not name.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase) AndAlso
                               Not String.Equals(name, "_._", StringComparison.Ordinal)
                       End Function) _
                .OrderBy(Function(file) file, StringComparer.OrdinalIgnoreCase)
        End Function

        ' ==================================================================
        ' nuspec 解析
        ' ==================================================================

        Private Class NuspecDescriptor
            Public Property Id As String
            Public Property Version As NuGetVersion
            Public ReadOnly Property DependencyGroups As New List(Of NuGetDependencyGroup)
        End Class

        Private Shared Function ParseNuspec(nuspecFile As String,
                                            fallbackId As String,
                                            fallbackVersion As NuGetVersion) As NuspecDescriptor

            Dim descriptor As New NuspecDescriptor With {
                .Id = fallbackId,
                .Version = fallbackVersion
            }

            Dim doc As XDocument = XDocument.Load(nuspecFile)
            Dim metadata As XElement = FindChild(doc.Root, "metadata")

            If metadata Is Nothing Then
                Return descriptor
            End If

            Dim idNode As XElement = FindChild(metadata, "id")

            If idNode IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(idNode.Value) Then
                descriptor.Id = idNode.Value.Trim()
            End If

            Dim versionNode As XElement = FindChild(metadata, "version")
            Dim parsed As NuGetVersion = Nothing

            If versionNode IsNot Nothing AndAlso NuGetVersion.TryParse(versionNode.Value, parsed) Then
                descriptor.Version = parsed
            End If

            Dim dependencies As XElement = FindChild(metadata, "dependencies")

            If dependencies Is Nothing Then
                Return descriptor
            End If

            Dim groups As XElement() = dependencies _
                .Elements() _
                .Where(Function(node) String.Equals(node.Name.LocalName, "group", StringComparison.OrdinalIgnoreCase)) _
                .ToArray()

            If groups.Length = 0 Then
                ' 平铺形式: <dependencies><dependency .../></dependencies>
                Dim fallbackGroup As New NuGetDependencyGroup With {.TargetFramework = ""}

                For Each dependency As NuGetDependency In ReadDependencies(dependencies)
                    Call fallbackGroup.Dependencies.Add(dependency)
                Next

                Call descriptor.DependencyGroups.Add(fallbackGroup)
                Return descriptor
            End If

            For Each group As XElement In groups
                Dim tfm As XAttribute = group.Attribute("targetFramework")
                Dim target As New NuGetDependencyGroup With {
                    .TargetFramework = If(tfm Is Nothing, "", tfm.Value.Trim())
                }

                For Each dependency As NuGetDependency In ReadDependencies(group)
                    Call target.Dependencies.Add(dependency)
                Next

                Call descriptor.DependencyGroups.Add(target)
            Next

            Return descriptor
        End Function

        Private Shared Function ReadDependencies(parent As XElement) As IEnumerable(Of NuGetDependency)
            Dim list As New List(Of NuGetDependency)

            For Each node As XElement In parent.Elements()
                If Not String.Equals(node.Name.LocalName, "dependency", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Dim id As XAttribute = node.Attribute("id")

                If id Is Nothing OrElse String.IsNullOrWhiteSpace(id.Value) Then
                    Continue For
                End If

                Dim exclude As XAttribute = node.Attribute("exclude")

                If exclude IsNot Nothing AndAlso
                    exclude.Value.Split(","c).Any(Function(token) String.Equals(token.Trim(), "all", StringComparison.OrdinalIgnoreCase)) Then

                    Continue For
                End If

                Dim version As XAttribute = node.Attribute("version")
                Dim range As VersionRange = If(
                    version Is Nothing OrElse String.IsNullOrWhiteSpace(version.Value),
                    VersionRange.All,
                    VersionRange.Parse(version.Value))

                Call list.Add(New NuGetDependency With {
                    .Id = id.Value.Trim(),
                    .Range = range
                })
            Next

            Return list
        End Function

        ''' <summary>按 localName 查找子元素(忽略 xml 命名空间差异)</summary>
        Private Shared Function FindChild(parent As XElement, localName As String) As XElement
            If parent Is Nothing Then
                Return Nothing
            End If

            Return parent _
                .Elements() _
                .FirstOrDefault(Function(node) String.Equals(node.Name.LocalName, localName, StringComparison.OrdinalIgnoreCase))
        End Function
    End Class
End Namespace
