' =====================================================================================
'  MOFA (Multi-Omics Factor Analysis) Framework - VB.NET Implementation
'  Based on: Argelaguet et al., Molecular Systems Biology 14:e8124 (2018)
'  
'  Core Model:  Y^m = Z * W^m^T + ε^m   for m = 1, ..., M views
'    - Y^m : N × D_m  data matrix for view m (samples × features)
'    - Z   : N × K    shared factor matrix (sample scores in factor space)
'    - W^m : D_m × K  weight matrix for view m (feature loadings)
'    - ε^m :          view-specific residual noise (Gaussian by default)
'  
'  Key Features Implemented:
'    1. Bayesian matrix factorization with ARD prior (auto factor selection)
'    2. Spike-and-slab prior for feature-wise sparsity (simplified)
'    3. Variational Bayesian inference (mean-field approximation)
'    4. ELBO monitoring for convergence
'    5. **Missing-sample handling via observation masks** (solves 3 vs 6 mismatch)
'    6. Automatic factor pruning based on variance-explained threshold
'    7. Variance-explained decomposition per view / per factor
'  
'  Author: Qingyan Agent (VB.NET port based on user's Tensor class)
'  License: GPL3 (consistent with original Tensor.vb)
' =====================================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace MultiOmics.MOFA

    ' =================================================================================
    '  DataView : Represents a single omics view with potentially missing samples
    '  - Each view has its own sample subset (key to handling 3 vs 6 mismatch)
    '  - Stores observation mask for missing-value-aware variational updates
    ' =================================================================================
    Public Class DataView

        ''' <summary>View name (e.g. "mRNA", "metabolome")</summary>
        Public Property Name As String

        ''' <summary>
        ''' Data matrix Y^m of shape (N_local × D_m), where N_local is the number of
        ''' samples actually observed in this view, and D_m is the number of features.
        ''' Rows correspond to samples listed in <see cref="SampleIds"/>.
        ''' </summary>
        Public Property Data As Tensor

        ''' <summary>
        ''' Sample IDs that are observed in this view (length = N_local = Data.Shape(0)).
        ''' These IDs map local rows to the global sample index space.
        ''' </summary>
        Public Property SampleIds As String()

        ''' <summary>Feature names for this view (length = D_m)</summary>
        Public Property FeatureNames As String()

        ''' <summary>
        ''' Local-to-global sample index mapping.
        ''' localToGlobal(i) = global index of the i-th observed sample in this view.
        ''' Built by <see cref="BuildIndexMapping"/> using the global sample list.
        ''' </summary>
        Public Property LocalToGlobal As Integer()

        ''' <summary>
        ''' Reverse mapping: globalToLocal(n) = local index of global sample n in this view,
        ''' or -1 if sample n is NOT observed in this view. This is the key data structure
        ''' that enables MOFA to handle mismatched sample sets between views.
        ''' </summary>
        Public Property GlobalToLocal As Integer()

        ''' <summary>Feature-wise mean (D_m) — subtracted before factorization</summary>
        Public Property FeatureMean As Double()

        ''' <summary>Feature-wise variance (D_m) — used for Z-score normalization</summary>
        Public Property FeatureStd As Double()

        ''' <summary>Number of features D_m</summary>
        Public ReadOnly Property D As Integer
            Get
                Return Data.Shape(1)
            End Get
        End Property

        ''' <summary>Number of locally observed samples</summary>
        Public ReadOnly Property NLocal As Integer
            Get
                Return Data.Shape(0)
            End Get
        End Property

        Public Sub New(name As String,
                       data As Tensor,
                       sampleIds As String(),
                       featureNames As String())
            Me.Name = name
            Me.Data = data
            Me.SampleIds = sampleIds
            Me.FeatureNames = featureNames
        End Sub

        ''' <summary>
        ''' Build the local-to-global sample index mapping using the global sample list.
        ''' Also builds the reverse global-to-local mapping, which is the key data
        ''' structure for handling mismatched sample sets between views.
        ''' Samples in this view that are not in the global list will be flagged -1
        ''' (and skipped during inference).
        ''' </summary>
        Public Sub BuildIndexMapping(globalSampleIds As String())
            Dim globalDict As New Dictionary(Of String, Integer)
            For i = 0 To globalSampleIds.Length - 1
                globalDict(globalSampleIds(i)) = i
            Next

            LocalToGlobal = New Integer(SampleIds.Length - 1) {}
            For i = 0 To SampleIds.Length - 1
                If globalDict.ContainsKey(SampleIds(i)) Then
                    LocalToGlobal(i) = globalDict(SampleIds(i))
                Else
                    LocalToGlobal(i) = -1 ' not in global space — should not happen if user prepared data correctly
                End If
            Next

            ' Build reverse mapping: globalToLocal(n) = local index, or -1 if not observed
            GlobalToLocal = New Integer(globalSampleIds.Length - 1) {}
            For n = 0 To globalSampleIds.Length - 1
                GlobalToLocal(n) = -1
            Next
            For i = 0 To SampleIds.Length - 1
                Dim gIdx = LocalToGlobal(i)
                If gIdx >= 0 Then
                    GlobalToLocal(gIdx) = i
                End If
            Next
        End Sub

        ''' <summary>
        ''' Center &amp; scale each feature to zero mean / unit variance (Z-score).
        ''' This is critical because different omics have vastly different scales
        ''' (e.g. RNA counts vs metabolite peak areas) — without it, high-variance
        ''' views would dominate the factor structure.
        ''' </summary>
        Public Sub Standardize()
            Dim N = Data.Shape(0)
            Dim D = Data.Shape(1)
            FeatureMean = New Double(D - 1) {}
            FeatureStd = New Double(D - 1) {}

            ' 1) Compute per-feature mean
            For D = 0 To D - 1
                Dim s = 0.0
                For N = 0 To N - 1
                    s += Data(N, D)
                Next
                FeatureMean(D) = s / N
            Next

            ' 2) Compute per-feature std
            For D = 0 To D - 1
                Dim s = 0.0
                For N = 0 To N - 1
                    Dim diff = Data(N, D) - FeatureMean(D)
                    s += diff * diff
                Next
                Dim var = s / std.Max(N - 1, 1)
                FeatureStd(D) = std.Sqrt(var)
                If FeatureStd(D) < 0.00000001 Then FeatureStd(D) = 1.0 ' guard against zero-variance features
            Next

            ' 3) Apply Z-score in place
            For N = 0 To N - 1
                For D = 0 To D - 1
                    Data(N, D) = (Data(N, D) - FeatureMean(D)) / FeatureStd(D)
                Next
            Next
        End Sub

    End Class

    ' =================================================================================
    '  MOFAOptions : Training hyperparameters
    ' =================================================================================
    Public Class MOFAOptions

        ''' <summary>Initial number of factors K (will be pruned during training)</summary>
        Public Property NumFactors As Integer = 15

        ''' <summary>Maximum number of variational iterations</summary>
        Public Property MaxIterations As Integer = 500

        ''' <summary>ELBO convergence tolerance (relative change)</summary>
        Public Property ConvergenceTolerance As Double = 0.0001

        ''' <summary>
        ''' Minimum fraction of variance explained for a factor to be retained.
        ''' Factors below this threshold are pruned. (Paper uses 2%.)
        ''' </summary>
        Public Property DropFactorThreshold As Double = 0.02

        ''' <summary>Number of initial "burn-in" iterations without sparsity (paper Sec. 5)</summary>
        Public Property DropIterations As Integer = 50

        ''' <summary>Random seed for reproducibility</summary>
        Public Property Seed As Integer? = 42

        ''' <summary>Verbose output during training</summary>
        Public Property Verbose As Boolean = True

        ''' <summary>Print ELBO every N iterations when verbose</summary>
        Public Property PrintEvery As Integer = 10

        ''' <summary>
        ''' Prior precision for the spike distribution (small value → strong sparsity).
        ''' Set to 0 to disable spike-and-slab feature sparsity (use ARD only).
        ''' </summary>
        Public Property SpikePrecision As Double = 0.01

        ''' <summary>Whether to standardize each view before training (recommended)</summary>
        Public Property StandardizeViews As Boolean = True

    End Class

    ' =================================================================================
    '  MOFA : The main Multi-Omics Factor Analysis model
    ' =================================================================================
    Public Class MOFA
        Implements IDisposable

        ' -------- Model parameters (variational posterior expectations) --------

        ''' <summary>Factor matrix Z (N × K) — shared across all views</summary>
        Public Property Z As Tensor

        ''' <summary>Weight matrices W^m (one per view), each (D_m × K)</summary>
        Public Property W As List(Of Tensor)

        ''' <summary>
        ''' Per-view, per-feature noise precision τ^m_d (D_m × 1 for each view).
        ''' Gaussian noise model: ε ~ N(0, 1/τ).
        ''' </summary>
        Public Property Tau As List(Of Double())

        ' -------- ARD prior parameters (control factor activation per view) --------

        ''' <summary>
        ''' ARD precision α^m_k (M × K). Large α → factor k is suppressed in view m.
        ''' Updated during inference; factors with large α across all views are pruned.
        ''' </summary>
        Public Property Alpha As Double(,)  ' (M, K)

        ' -------- Spike-and-slab parameters (feature-wise sparsity) --------

        ''' <summary>
        ''' Per-feature spike-and-slab mixing probability π^m_d ∈ [0,1] (D_m × 1 per view).
        ''' π close to 1 → feature d is "active" (drawn from slab);
        ''' π close to 0 → feature d is "inactive" (drawn from spike).
        ''' </summary>
        Public Property Pi As List(Of Double())

        ' -------- Training state --------

        Public Property Options As MOFAOptions
        Public Property Views As List(Of DataView)
        Public Property GlobalSampleIds As String()

        ''' <summary>Number of global samples (union of all views' samples)</summary>
        Public Property N As Integer

        ''' <summary>Number of views M</summary>
        Public Property M As Integer

        ''' <summary>Current number of active factors K (may decrease due to pruning)</summary>
        Public Property K As Integer

        ''' <summary>Active factor indices (after pruning)</summary>
        Public Property ActiveFactors As Boolean()

        ''' <summary>ELBO history (one entry per iteration)</summary>
        Public Property ElboHistory As New List(Of Double)

        ''' <summary>Whether training has converged</summary>
        Public Property Converged As Boolean = False

        Private _rng As Random
        Private _disposed As Boolean = False

        ' =================================================================================
        '  Construction & data preparation
        ' =================================================================================

        Public Sub New(views As List(Of DataView), Optional options As MOFAOptions = Nothing)
            Me.Views = views
            Me.Options = If(options, New MOFAOptions())
            Me.M = views.Count

            ' Build the global sample space (union of all view sample IDs, preserving first-seen order)
            Dim globalSet As New HashSet(Of String)
            Dim globalList As New List(Of String)
            For Each v In views
                For Each s In v.SampleIds
                    If globalSet.Add(s) Then globalList.Add(s)
                Next
            Next
            Me.GlobalSampleIds = globalList.ToArray()
            Me.N = GlobalSampleIds.Length

            ' Build local-to-global index mapping for each view
            For Each v In views
                v.BuildIndexMapping(GlobalSampleIds)
            Next

            ' Standardize each view if requested
            If Me.Options.StandardizeViews Then
                For Each v In views
                    v.Standardize()
                Next
            End If

            ' Initialize RNG
            _rng = If(Me.Options.Seed.HasValue, New Random(Me.Options.Seed.Value), New Random())
        End Sub

        ' =================================================================================
        '  Model initialization
        ' =================================================================================

        ''' <summary>
        ''' Initialize all variational parameters.
        ''' Z is initialized from a small random normal; W from PCA-like SVD on each view
        ''' (here we use random init for simplicity, with multiple restarts recommended).
        ''' </summary>
        Public Sub Initialize()
            Call Console.WriteLine("Initialize all variational parameters.")

            K = Options.NumFactors
            ActiveFactors = New Boolean(K - 1) {}
            For K As Integer = 0 To Me.K - 1
                ActiveFactors(K) = True
            Next

            ' --- Initialize Z ~ N(0, 1) (prior is standard normal) ---
            Z = New Tensor(N, K)
            For N As Integer = 0 To Me.N - 1
                For K As Integer = 0 To Me.K - 1
                    Z(N, K) = SampleNormal(0.0, 1.0)
                Next
            Next

            ' --- Initialize W^m for each view (small random) ---
            W = New List(Of Tensor)()
            Tau = New List(Of Double())()
            Pi = New List(Of Double())()
            For M As Integer = 0 To Me.M - 1
                Dim Dm = Views(M).D
                Dim Wm = New Tensor(Dm, K)
                For d = 0 To Dm - 1
                    For K As Integer = 0 To Me.K - 1
                        Wm(d, K) = SampleNormal(0.0, 0.1)
                    Next
                Next
                W.Add(Wm)

                ' Initialize noise precision τ = 1 (i.e. variance = 1)
                Dim tauM = New Double(Dm - 1) {}
                For d = 0 To Dm - 1
                    tauM(d) = 1.0
                Next
                Tau.Add(tauM)

                ' Initialize π = 0.5 (uninformative)
                Dim piM = New Double(Dm - 1) {}
                For d = 0 To Dm - 1
                    piM(d) = 0.5
                Next
                Pi.Add(piM)
            Next

            ' --- Initialize ARD parameters α^m_k = 1 (uninformative) ---
            Alpha = New Double(M - 1, K - 1) {}
            For M As Integer = 0 To Me.M - 1
                For K As Integer = 0 To Me.K - 1
                    Alpha(M, K) = 1.0
                Next
            Next
        End Sub

        ' =================================================================================
        '  Variational Bayesian Inference — main training loop
        ' =================================================================================

        ''' <summary>
        ''' Train the MOFA model using variational Bayes.
        ''' Each iteration performs:
        '''   1. Update Z (factors) given W, τ, α
        '''   2. Update W^m (weights) given Z, τ, α, π (per view)
        '''   3. Update τ^m (noise precision) per view
        '''   4. Update α^m_k (ARD parameters) per view/factor
        '''   5. Update π^m_d (spike-slab mixing) per view/feature (after burn-in)
        '''   6. Compute ELBO; check convergence
        '''   7. Prune inactive factors (every DropIterations after burn-in)
        ''' </summary>
        Public Sub Train()
            If Z Is Nothing Then
                Initialize()
            End If

            ElboHistory.Clear()
            Converged = False

            If Options.Verbose Then
                Console.WriteLine($"[MOFA] Starting training: N={N} samples, M={M} views, K={K} factors")
                For M As Integer = 0 To Me.M - 1
                    Console.WriteLine($"   View {M} ({Views(M).Name}): N_local={Views(M).NLocal}, D={Views(M).D}")
                Next
                Console.WriteLine()
            End If

            Dim prevElbo As Double = Double.NaN
            Dim bar As ProgressBar = Nothing

            For Each iter In TqdmWrapper.Range(0, Options.MaxIterations, bar:=bar)
                ' 1) Update factors Z
                UpdateFactors()

                ' 2) Update weights W^m for each view
                For M As Integer = 0 To Me.M - 1
                    UpdateWeights(M)
                Next

                ' 3) Update noise precision τ^m for each view
                For M As Integer = 0 To Me.M - 1
                    UpdateNoisePrecision(M)
                Next

                ' 4) Update ARD parameters α^m_k
                For M As Integer = 0 To Me.M - 1
                    UpdateAlpha(M)
                Next

                ' 5) Update spike-and-slab π^m_d (only after burn-in)
                If iter > Options.DropIterations Then
                    For M As Integer = 0 To Me.M - 1
                        UpdatePi(M)
                    Next
                End If

                ' 6) Compute ELBO
                Dim elbo = ComputeELBO()
                ElboHistory.Add(elbo)

                ' Convergence check
                If iter > 1 Then
                    Dim relChange = std.Abs((elbo - prevElbo) / std.Abs(prevElbo))
                    If relChange < Options.ConvergenceTolerance Then
                        Converged = True
                        If Options.Verbose Then
                            bar.SetLabel($"[MOFA] Converged at iteration {iter} (ELBO={elbo:F4})")
                        End If
                        Exit For
                    End If
                End If
                prevElbo = elbo

                ' 7) Factor pruning (after burn-in, every DropIterations)
                If iter > Options.DropIterations AndAlso iter Mod Options.DropIterations = 0 Then
                    PruneFactors()
                End If

                ' Verbose output
                If Options.Verbose AndAlso iter Mod Options.PrintEvery = 0 Then
                    bar.SetLabel($"[MOFA] Iter {iter,4} | ELBO={elbo,12:F4} | K_active={CountActiveFactors()}")
                End If
            Next

            If Not Converged AndAlso Options.Verbose Then
                Console.WriteLine($"[MOFA] Reached max iterations ({Options.MaxIterations}). Final ELBO={ElboHistory.Last():F4}")
            End If

            ' Final pruning pass
            PruneFactors()
        End Sub

        ' =================================================================================
        '  Variational update for factors Z (N × K)
        '  
        '  Posterior for Z is Gaussian with:
        '    E[Z_n,k] = ( Σ_m τ^m_d * W^m^T * (Y^m_n - μ^m) ) / ( 1 + Σ_m α^m_k * ||W^m_k||² )
        '  where the sums are over observed features in each view.
        '  
        '  Note: Z is shared across all views, so each sample n's posterior uses
        '  information from ALL views in which n is observed. Samples not observed
        '  in any view remain at their prior (≈0).
        '  
        '  This is the KEY mechanism that handles mismatched sample sets:
        '  - Sample n's Z_n is updated using only the views where n is observed
        '  - The GlobalToLocal mapping provides O(1) lookup
        ' =================================================================================
        Private Sub UpdateFactors()
            For N As Integer = 0 To Me.N - 1
                For K As Integer = 0 To Me.K - 1
                    If Not ActiveFactors(K) Then Continue For

                    Dim numerator = 0.0
                    Dim denominator = 1.0  ' prior precision (standard normal prior)

                    For M As Integer = 0 To Me.M - 1
                        Dim view = Views(M)
                        ' O(1) lookup: is sample n observed in view m?
                        Dim localIdx = view.GlobalToLocal(N)
                        If localIdx < 0 Then Continue For ' sample n not observed in view m

                        Dim Wm = W(M)
                        Dim tauM = Tau(M)
                        Dim Dm = view.D

                        ' Σ_d τ_d * W_d,k * Y_n,d  +  Σ_d τ_d * W_d,k² * Z_n,k
                        ' Since we already centered the data, μ_d = 0.
                        ' Solve: Z_n,k = num / denom where
                        '   num   = Σ_d τ_d * W_d,k * Y_n,d
                        '   denom = 1 (prior) + Σ_m Σ_d τ_d * W_d,k²
                        For d = 0 To Dm - 1
                            Dim Wdk = Wm(d, K)
                            Dim Ynd = view.Data(localIdx, d)
                            numerator += tauM(d) * Wdk * Ynd
                            denominator += tauM(d) * Wdk * Wdk
                        Next
                    Next

                    If denominator < 0.0000000001 Then denominator = 0.0000000001
                    Z(N, K) = numerator / denominator
                Next
            Next
        End Sub

        ''' <summary>
        '''  Variational update for weights W^m (D_m × K) in view m
        '''  
        '''  Posterior for W^m_d,k is Gaussian with:
        '''    E[W^m_d,k] = ( Σ_n τ_d * Z_n,k * (Y^m_n,d - μ_d) ) / ( α^m_k + τ_d * Σ_n Z_n,k² )
        '''  where the sum over n only includes samples observed in view m.
        '''  
        '''  Spike-and-slab: if π^m_d is small, the effective precision is dominated by
        '''  the spike (large regularization), pushing W toward 0.
        ''' </summary>
        ''' <param name="m"></param>
        Private Sub UpdateWeights(m As Integer)
            Dim view = Views(m)
            Dim Dm = view.D
            Dim Wm = W(m)
            Dim tauM = Tau(m)
            Dim piM = Pi(m)

            ' Precompute Σ_n Z_n,k * Z_n,k' over observed samples in this view
            ' (used for the posterior covariance of W)
            Dim ZtZ(K - 1, K - 1) As Double
            For k1 = 0 To K - 1
                For k2 = 0 To K - 1
                    Dim s = 0.0
                    For i = 0 To view.NLocal - 1
                        Dim n = view.LocalToGlobal(i)
                        If n < 0 Then Continue For
                        s += Z(n, k1) * Z(n, k2)
                    Next
                    ZtZ(k1, k2) = s
                Next
            Next

            For d = 0 To Dm - 1
                Dim tauD = tauM(d)

                ' Compute Σ_n Z_n,k * Y_n,d over observed samples
                Dim ZtY(K - 1) As Double
                For K As Integer = 0 To Me.K - 1
                    Dim s = 0.0
                    For i = 0 To view.NLocal - 1
                        Dim n = view.LocalToGlobal(i)
                        If n < 0 Then Continue For
                        s += Z(n, K) * view.Data(i, d)
                    Next
                    ZtY(K) = s
                Next

                ' Spike-and-slab effective precision:
                ' If π_d is small, the feature is "in the spike" → strong regularization.
                ' We model this by inflating the prior precision α by (1-π)/spikePrecision.
                Dim spikeEffect = 1.0
                If Options.SpikePrecision > 0 Then
                    Dim piD = piM(d)
                    ' Effective ARD precision: blend between slab (α) and spike (1/spikePrecision)
                    spikeEffect = piD + (1.0 - piD) * (Options.SpikePrecision * 100.0) ' heuristic scaling
                End If

                ' Update each W_d,k via coordinate ascent (diagonal covariance approximation)
                For K As Integer = 0 To Me.K - 1
                    If Not ActiveFactors(K) Then
                        Wm(d, K) = 0.0
                        Continue For
                    End If

                    Dim denom = Alpha(m, K) * spikeEffect + tauD * ZtZ(K, K)
                    If denom < 0.0000000001 Then denom = 0.0000000001

                    ' Subtract contributions from other factors (Gauss-Seidel style)
                    Dim otherContrib = 0.0
                    For kOther = 0 To K - 1
                        If kOther = K OrElse Not ActiveFactors(kOther) Then Continue For
                        otherContrib += ZtZ(K, kOther) * Wm(d, kOther)
                    Next

                    Dim numer = ZtY(K) - tauD * otherContrib
                    Wm(d, K) = numer / denom
                Next
            Next
        End Sub

        '  Update noise precision τ^m_d for view m (Gamma posterior)
        '''  
        '''  τ^m_d ~ Gamma(a0 + N_obs/2, b0 + 0.5 * Σ_n (Y_n,d - Σ_k Z_n,k * W_d,k)²)
        '''  With uninformative prior (a0=b0=0), the posterior mean is:
        '''    τ^m_d = N_obs / Σ_n residual²
        Private Sub UpdateNoisePrecision(m As Integer)
            Dim view = Views(m)
            Dim Dm = view.D
            Dim Wm = W(m)
            Dim tauM = Tau(m)
            Dim Nobs = view.NLocal

            For d = 0 To Dm - 1
                Dim ssr = 0.0  ' sum of squared residuals
                For i = 0 To Nobs - 1
                    Dim n = view.LocalToGlobal(i)
                    If n < 0 Then Continue For

                    Dim pred = 0.0
                    For K As Integer = 0 To Me.K - 1
                        If Not ActiveFactors(K) Then Continue For
                        pred += Z(n, K) * Wm(d, K)
                    Next
                    Dim resid = view.Data(i, d) - pred
                    ssr += resid * resid
                Next

                ' Posterior mean of τ (with weak prior a0=1e-2, b0=1e-2 to avoid div-by-zero)
                Dim a0 = 0.01
                Dim b0 = 0.01
                Dim aPost = a0 + Nobs / 2.0
                Dim bPost = b0 + ssr / 2.0
                tauM(d) = aPost / bPost
            Next
        End Sub

        '  Update ARD parameters α^m_k for view m
        '''  
        '''  α^m_k ~ Gamma(c0 + D_m/2, d0 + 0.5 * Σ_d W^m_d,k²)
        '''  With uninformative prior, posterior mean:
        '''    α^m_k = D_m / Σ_d W^m_d,k²
        '''  Large α → factor k is suppressed in view m.
        Private Sub UpdateAlpha(m As Integer)
            Dim view = Views(m)
            Dim Dm = view.D
            Dim Wm = W(m)

            For K As Integer = 0 To Me.K - 1
                If Not ActiveFactors(K) Then Continue For

                Dim sumSq = 0.0
                For d = 0 To Dm - 1
                    sumSq += Wm(d, K) * Wm(d, K)
                Next

                ' Weak Gamma prior (c0=d0=1e-2)
                Dim c0 = 0.01
                Dim d0 = 0.01
                Dim cPost = c0 + Dm / 2.0
                Dim dPost = d0 + sumSq / 2.0
                Alpha(m, K) = cPost / dPost
            Next
        End Sub

        '  Update spike-and-slab mixing π^m_d for view m
        '''  
        '''  π^m_d = sigmoid( log(π_prior/(1-π_prior)) + 0.5*log(τ_d/spikePrecision)
        '''                   + 0.5*τ_d*||W_d||² - 0.5*spikePrecision*||W_d||² )
        '''  Simplified: π_d reflects whether the feature's weights are large enough
        '''  to favor the slab over the spike.
        Private Sub UpdatePi(m As Integer)
            Dim view = Views(m)
            Dim Dm = view.D
            Dim Wm = W(m)
            Dim tauM = Tau(m)
            Dim piM = Pi(m)
            Dim spike = Options.SpikePrecision

            For d = 0 To Dm - 1
                Dim wNormSq = 0.0
                For K As Integer = 0 To Me.K - 1
                    If Not ActiveFactors(K) Then Continue For
                    wNormSq += Wm(d, K) * Wm(d, K)
                Next

                ' Log-odds in favor of slab:
                ' logit(π) = log(π0/(1-π0)) + 0.5*(τ_d - spike)*||W_d||² + 0.5*log(τ_d/spike)
                Dim logitPi = std.Log(0.5 / 0.5) + 0.5 * (tauM(d) - spike) * wNormSq + 0.5 * std.Log(tauM(d) / spike)
                ' Sigmoid
                piM(d) = 1.0 / (1.0 + std.Exp(-logitPi))
            Next
        End Sub

        '  Compute the Evidence Lower Bound (ELBO)
        '''  
        '''  ELBO = E[log p(Y|Z,W,τ)] - E[log q(Z,W,τ,α,π)] + E[log p(Z,W,τ,α,π)]
        '''  
        '''  We use a simplified form that captures the dominant terms:
        '''    ELBO ≈ -0.5 * Σ_m Σ_n Σ_d τ_d * (Y_n,d - Σ_k Z_n,k * W_d,k)²
        '''           -0.5 * Σ_n ||Z_n||²                                    (prior on Z)
        '''           -0.5 * Σ_m Σ_d Σ_k α_k * W_d,k²                        (ARD prior on W)
        '''           + Σ_m Σ_d log(τ_d)                                      (entropy of τ)
        '''           - Σ_m Σ_k log(α_k)                                      (entropy of α)
        '''  
        '''  This is a lower bound; the absolute value is not meaningful, only the trend.
        Private Function ComputeELBO() As Double
            Dim elbo = 0.0

            ' 1) Reconstruction term: -0.5 * Σ τ * residual²
            For M As Integer = 0 To Me.M - 1
                Dim view = Views(M)
                Dim Wm = W(M)
                Dim tauM = Tau(M)
                For i = 0 To view.NLocal - 1
                    Dim n = view.LocalToGlobal(i)
                    If n < 0 Then Continue For
                    For d = 0 To view.D - 1
                        Dim pred = 0.0
                        For K As Integer = 0 To Me.K - 1
                            If Not ActiveFactors(K) Then Continue For
                            pred += Z(n, K) * Wm(d, K)
                        Next
                        Dim resid = view.Data(i, d) - pred
                        elbo -= 0.5 * tauM(d) * resid * resid
                    Next
                Next
            Next

            ' 2) Prior on Z: -0.5 * Σ ||Z_n||²
            For N As Integer = 0 To Me.N - 1
                For K As Integer = 0 To Me.K - 1
                    If Not ActiveFactors(K) Then Continue For
                    elbo -= 0.5 * Z(N, K) * Z(N, K)
                Next
            Next

            ' 3) ARD prior on W: -0.5 * Σ_m Σ_d Σ_k α_k * W_d,k²
            For M As Integer = 0 To Me.M - 1
                Dim Wm = W(M)
                For d = 0 To Wm.Shape(0) - 1
                    For K As Integer = 0 To Me.K - 1
                        If Not ActiveFactors(K) Then Continue For
                        elbo -= 0.5 * Alpha(M, K) * Wm(d, K) * Wm(d, K)
                    Next
                Next
            Next

            ' 4) Entropy of τ (log τ terms)
            For M As Integer = 0 To Me.M - 1
                Dim tauM = Tau(M)
                For d = 0 To tauM.Length - 1
                    elbo += 0.5 * std.Log(tauM(d) + 0.0000000001)
                Next
            Next

            ' 5) Entropy of α (log α terms)
            For M As Integer = 0 To Me.M - 1
                For K As Integer = 0 To Me.K - 1
                    If Not ActiveFactors(K) Then Continue For
                    elbo -= 0.5 * std.Log(Alpha(M, K) + 0.0000000001)
                Next
            Next

            Return elbo
        End Function

        '  Factor pruning — deactivate factors with low variance explained
        '''  
        '''  A factor k is pruned if, across ALL views, its variance explained is below
        '''  the threshold (default 2%). This implements the automatic factor selection
        '''  described in the paper (Model training and selection section).
        Private Sub PruneFactors()
            For K As Integer = 0 To Me.K - 1
                If Not ActiveFactors(K) Then Continue For

                Dim totalVarExplained = 0.0
                Dim viewCount = 0
                For M As Integer = 0 To Me.M - 1
                    Dim ve = ComputeVarianceExplained(M, K)
                    totalVarExplained += ve
                    viewCount += 1
                Next

                Dim avgVE = totalVarExplained / std.Max(viewCount, 1)
                If avgVE < Options.DropFactorThreshold Then
                    ActiveFactors(K) = False
                    ' Zero out the corresponding columns of Z and W
                    For N As Integer = 0 To Me.N - 1
                        Z(N, K) = 0.0
                    Next
                    For M As Integer = 0 To Me.M - 1
                        Dim Wm = W(M)
                        For d = 0 To Wm.Shape(0) - 1
                            Wm(d, K) = 0.0
                        Next
                    Next
                    If Options.Verbose Then
                        Console.WriteLine($"[MOFA] Pruned factor {K} (avg VE = {avgVE:P})")
                    End If
                End If
            Next
        End Sub

        ''' <summary>Count currently active factors</summary>
        Private Function CountActiveFactors() As Integer
            Dim count = 0
            For K As Integer = 0 To Me.K - 1
                If ActiveFactors(K) Then count += 1
            Next
            Return count
        End Function

        ' =================================================================================
        '  Downstream analysis utilities
        ' =================================================================================

        ''' <summary>
        ''' Compute the fraction of variance explained (R²) by factor k in view m.
        '''   R²_{m,k} = 1 - Σ_{n,d}(Y - Z_k*W_k)² / Σ_{n,d} Y²
        ''' (Data is already centered, so μ_d = 0.)
        ''' </summary>
        Public Function ComputeVarianceExplained(m As Integer, k As Integer) As Double
            Dim view = Views(m)
            Dim Wm = W(m)
            Dim ssResid = 0.0
            Dim ssTotal = 0.0

            For i = 0 To view.NLocal - 1
                Dim n = view.LocalToGlobal(i)
                If n < 0 Then Continue For
                For d = 0 To view.D - 1
                    Dim pred = Z(n, k) * Wm(d, k)
                    Dim resid = view.Data(i, d) - pred
                    ssResid += resid * resid
                    ssTotal += view.Data(i, d) * view.Data(i, d)
                Next
            Next

            If ssTotal < 0.0000000001 Then Return 0.0
            Return 1.0 - ssResid / ssTotal
        End Function

        ''' <summary>
        ''' Compute the total fraction of variance explained by ALL active factors in view m.
        ''' </summary>
        Public Function ComputeTotalVarianceExplained(m As Integer) As Double
            Dim view = Views(m)
            Dim Wm = W(m)
            Dim ssResid = 0.0
            Dim ssTotal = 0.0

            For i = 0 To view.NLocal - 1
                Dim n = view.LocalToGlobal(i)
                If n < 0 Then Continue For
                For d = 0 To view.D - 1
                    Dim pred = 0.0
                    For K As Integer = 0 To Me.K - 1
                        If Not ActiveFactors(K) Then Continue For
                        pred += Z(n, K) * Wm(d, K)
                    Next
                    Dim resid = view.Data(i, d) - pred
                    ssResid += resid * resid
                    ssTotal += view.Data(i, d) * view.Data(i, d)
                Next
            Next

            If ssTotal < 0.0000000001 Then Return 0.0
            Return 1.0 - ssResid / ssTotal
        End Function

        ''' <summary>
        ''' Reconstruct (impute) the full data matrix for view m, including samples
        ''' that were NOT observed in view m. This is the key output for the user's
        ''' 3-vs-6 mismatch problem: missing samples get imputed factor-based predictions.
        ''' </summary>
        Public Function ReconstructView(m As Integer) As Tensor
            Dim view = Views(m)
            Dim Wm = W(m)
            Dim Dm = view.D
            Dim result = New Tensor(N, Dm)

            For N As Integer = 0 To Me.N - 1
                For d = 0 To Dm - 1
                    Dim pred = 0.0
                    For K As Integer = 0 To Me.K - 1
                        If Not ActiveFactors(K) Then Continue For
                        pred += Z(N, K) * Wm(d, K)
                    Next
                    ' Un-standardize back to original scale
                    result(N, d) = pred * view.FeatureStd(d) + view.FeatureMean(d)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' Get the factor scores Z as a (N × K_active) Tensor, dropping pruned factors.
        ''' </summary>
        Public Function GetFactors() As Tensor
            Dim kActive = CountActiveFactors()
            Dim result = New Tensor(N, kActive)
            Dim col = 0
            For K As Integer = 0 To Me.K - 1
                If Not ActiveFactors(K) Then Continue For
                For N As Integer = 0 To Me.N - 1
                    result(N, col) = Z(N, K)
                Next
                col += 1
            Next
            Return result
        End Function

        ''' <summary>
        ''' Get the weight matrix W^m as a (D_m × K_active) Tensor, dropping pruned factors.
        ''' </summary>
        Public Function GetWeights(m As Integer) As Tensor
            Dim view = Views(m)
            Dim kActive = CountActiveFactors()
            Dim result = New Tensor(view.D, kActive)
            Dim col = 0
            For K As Integer = 0 To Me.K - 1
                If Not ActiveFactors(K) Then Continue For
                For d = 0 To view.D - 1
                    result(d, col) = W(m)(d, K)
                Next
                col += 1
            Next
            Return result
        End Function

        ' =================================================================================
        '  Helpers
        ' =================================================================================

        Private Function SampleNormal(mean As Double, stdDev As Double) As Double
            ' Box-Muller transform
            Dim u1 = 1.0 - _rng.NextDouble()
            Dim u2 = 1.0 - _rng.NextDouble()
            Dim randStdNormal = std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
            Return mean + stdDev * randStdNormal
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                If Z IsNot Nothing Then Z.Dispose()
                If W IsNot Nothing Then
                    For Each W As Tensor In Me.W
                        W.Dispose()
                    Next
                End If
                _disposed = True
            End If
        End Sub

        Protected Overrides Sub Finalize()
            Dispose()
        End Sub

    End Class

End Namespace
