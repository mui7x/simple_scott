Imports Microsoft.VisualBasic

Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes
Imports System.Windows.Threading
Imports ScottPlot

Partial Public Class LiveDataFixed
    Inherits Window

    Private rand As New Random()
    Private liveData As Double() = New Double(399) {}
    Private ecg As New DataGen.Electrocardiogram()
    Private sw As Stopwatch = Stopwatch.StartNew()

    Private _updateDataTimer As Timer
    Private _renderTimer As DispatcherTimer

    Public Sub New()
        InitializeComponent() ' This line was missing

        ' plot the data array only once
        wpfPlot1.Plot.AddSignal(liveData)
        wpfPlot1.Plot.AxisAutoX(margin:=0)
        wpfPlot1.Plot.SetAxisLimits(yMin:=-1, yMax:=2.5)
        wpfPlot1.Refresh()

        ' create a traditional timer to update the data
        _updateDataTimer = New Timer(Sub(state) UpdateData(), Nothing, 0, 5)

        ' create a separate timer to update the GUI
        _renderTimer = New DispatcherTimer()
        _renderTimer.Interval = TimeSpan.FromMilliseconds(10)
        AddHandler _renderTimer.Tick, AddressOf Render
        _renderTimer.Start()

        AddHandler Closed, Sub(sender, args)
                               _updateDataTimer?.Dispose()
                               _renderTimer?.[Stop]()
                           End Sub
    End Sub

    Private Sub UpdateData()
        ' "scroll" the whole chart to the left
        Array.Copy(liveData, 1, liveData, 0, liveData.Length - 1)

        ' place the newest data point at the end
        Dim nextValue As Double = ecg.GetVoltage(sw.Elapsed.TotalSeconds)
        liveData(liveData.Length - 1) = nextValue
    End Sub

    Private Sub Render(sender As Object, e As EventArgs)
        wpfPlot1.Refresh()
    End Sub
End Class

