Imports CoreAudio
Imports System.Collections.Generic

''' <summary>
''' Provides support for the new set of mixer APIs available under Windows Vista and later version of Windows.
''' </summary>
''' <remarks>
''' <p>This library is based on the work by Ray Molenkamp and completed by Xavier Flix to include the missing interfaces that allow for full enumeration of all the mixers and their lines and controls as well as full support to enumerate (and manipulate) any available sessions.</p>
''' <p>The way the class has been implemented is very similar to how the the legacy <see cref="CMixerPro">CMixerPro</see> class works, allowing for an easy "upgrade" for those who are already familiar with the inner workings of MixerProNET.</p>
''' <p>The functionality in the CCoreAudio class is provided through the CoreAudio.dll which is a .NET-based wrapper, written in c#, that provides access to many of the interfaces exposed by the Core Audio API.</p>
''' <p>MixerProNET's CCoreAudio class provides an abstraction layer to facilitate the usage of the Core Audio APIs but if you would like to have direct access to the CoreAudio.dll library, you are free to do so.
''' Actually, this library will be continuously developed (until all interfaces are implemented) and provided as a separate (and free) download at the <see href="http://software.xfx.net/netcl/mxp/">MixerProNET's web site</see>.</p>
''' <p>Note that in order to be able to use this class, your application will need to reference both MixerProNET.dll as well as CoreAudio.dll</p>
''' <p>Update (3/30/2011): Added support to change the default audio device using <see href="http://eretik.omegahg.com/">EreTIk's IPolicyConfig</see> class.</p>
''' </remarks>
Public Class CCoreAudio
    Implements IDisposable

    Private mMixers As New List(Of CMixer)

    ''' <summary>
    ''' Use this property to query whether CoreAudio is required.
    ''' </summary>
    ''' <returns>Returns True if Windows Vista or later is detected.</returns>
    Public Shared ReadOnly Property RequiresCoreAudio() As Boolean
        Get
            Return Environment.OSVersion.Version.Major >= 6
        End Get
    End Property

    ''' <summary>
    ''' Creates a new instance of the CCoreAudio class.
    ''' </summary>
    ''' <param name="mixerPro">It is required to pass an instance of the <see cref="CMixerPro">CMixerPro</see> class in order to instantiate the CCoreAudio class.</param>
    ''' <remarks></remarks>
    Public Sub New(mixerPro As CMixerPro)
        If mixerPro Is Nothing Then Throw New Exception("MixerProNET must be initialized before using CoreAudio")
        If Not CCoreAudio.RequiresCoreAudio Then Throw New Exception("Unsupported Windows version. CoreAudio support is only available under Windows Vista and later versions")

        EnumDevices(DataFlow.Render)
        EnumDevices(DataFlow.Capture)
    End Sub

    ''' <summary>
    ''' Provides access to all the available <see cref="CMixer">mixers</see> in the host computer, even if disabled.
    ''' </summary>
    ''' <remarks>
    ''' <p>The list includes all the mixers available on the host machine.</p>
    ''' <seealso cref="CMixer">CMixer</seealso>
    ''' </remarks>
    Public ReadOnly Property Mixers As List(Of CMixer)
        Get
            Return mMixers
        End Get
    End Property

    Private Sub EnumDevices(flow As DataFlow)
        Dim mmde As New MMDeviceEnumerator(Guid.NewGuid())
        Dim devCol = mmde.EnumerateAudioEndPoints(flow, DeviceState.Active Or DeviceState.Disabled Or DeviceState.Unplugged)

        For d As Integer = 0 To devCol.Count - 1
            mMixers.Add(New CMixer(devCol(d), flow))
        Next
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            For Each m As CMixer In mMixers
                m.Dispose()
            Next
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub
#End Region
End Class
