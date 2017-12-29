Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.VisualBasic.RSystem
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace survival

    Public Enum SurvTypes
        right
        left
        interval
        counting
        interval2
        mstate
    End Enum

    Public Module survival

        ''' <summary>
        ''' Create a survival object, usually used as a response variable in a model formula. 
        ''' Argument matching is special for this function, see Details below.
        ''' </summary>
        ''' <param name="time$">For right censored data, this Is the follow up time. For interval data, 
        ''' the first argument Is the starting time for the interval.</param>
        ''' <param name="time2$">ending time of the interval for interval censored or counting process data only. 
        ''' Intervals are assumed to be open on the left and closed on the right, (start, end]. For 
        ''' counting process data, event indicates whether an event occurred at the end of the interval.</param>
        ''' <param name="event$">The status indicator, normally 0=alive, 1=dead. Other choices are 
        ''' TRUE/FALSE (TRUE = death) or 1/2 (2=death). For interval censored data, the status 
        ''' indicator is 0=right censored, 1=event at time, 2=left censored, 3=interval censored. 
        ''' Although unusual, the event indicator can be omitted, in which case all subjects are 
        ''' assumed to have an event.</param>
        ''' <param name="type">character string specifying the type of censoring. Possible values are 
        ''' "right", "left", "counting", "interval", "interval2" or "mstate".</param>
        ''' <param name="origin#">for counting process data, the hazard function origin. This option 
        ''' was intended to be used in conjunction with a model containing time dependent strata in 
        ''' order to align the subjects properly when they cross over from one strata to another, 
        ''' but it has rarely proven useful.</param>
        ''' <returns></returns>
        Public Function Surv(time$, time2$, event$, Optional type As SurvTypes = SurvTypes.right,
    Optional origin# = 0) As String
            SyncLock R
                With R
                    Dim var$ = App.NextTempName

                    .call = $"{var} <- Surv({time}, {time2}, {event$},
    type={type.Rstring},
    origin={origin});"

                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Create a survival object, usually used as a response variable in a model formula. 
        ''' Argument matching is special for this function, see Details below.
        ''' </summary>
        ''' <param name="time$">For right censored data, this Is the follow up time. For interval data, 
        ''' the first argument Is the starting time for the interval.</param>
        ''' <param name="event$">The status indicator, normally 0=alive, 1=dead. Other choices are 
        ''' TRUE/FALSE (TRUE = death) or 1/2 (2=death). For interval censored data, the status 
        ''' indicator is 0=right censored, 1=event at time, 2=left censored, 3=interval censored. 
        ''' Although unusual, the event indicator can be omitted, in which case all subjects are 
        ''' assumed to have an event.</param>
        ''' <param name="type">character string specifying the type of censoring. Possible values are 
        ''' "right", "left", "counting", "interval", "interval2" or "mstate".</param>
        ''' <param name="origin#">for counting process data, the hazard function origin. This option 
        ''' was intended to be used in conjunction with a model containing time dependent strata in 
        ''' order to align the subjects properly when they cross over from one strata to another, 
        ''' but it has rarely proven useful.</param>
        ''' <returns></returns>
        Public Function Surv(time$, event$, Optional type As SurvTypes = SurvTypes.right,
    Optional origin# = 0) As String
            SyncLock R
                With R
                    Dim var$ = App.NextTempName

                    .call = $"{var} <- Surv({time}, {event$},
    type={type.Rstring},
    origin={origin});"

                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' In the case of is.Surv, a logical value TRUE if x inherits from class "Surv", otherwise an FALSE.
        ''' </summary>
        ''' <param name="x$">any R object.</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function isSurv(x$) As Boolean
            SyncLock R
                With R
                    Return .Evaluate($"is.Surv({x})").AsLogical.ToArray.First
                End With
            End SyncLock
        End Function
    End Module
End Namespace