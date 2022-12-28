Module modBitBlt
    Private Declare Auto Function BitBlt Lib "GDI32.DLL" (ByVal hdcDest As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As Int32) As Boolean

    Public Function copyRect(ByVal src As Control, ByVal rect As RectangleF) As Bitmap
        'Get a Graphics Object from the form
        Dim srcPic As Graphics = src.CreateGraphics

        'Create a EMPTY bitmap from that graphics
        Dim srcBmp As New Bitmap(src.Width, src.Height, srcPic)
        'Create a Graphics object in memory from that bitmap
        Dim srcMem As Graphics = Graphics.FromImage(srcBmp)

        'get the IntPtr's of the graphics
        Dim HDC1 As IntPtr = srcPic.GetHdc
        'get the IntPtr's of the graphics
        Dim HDC2 As IntPtr = srcMem.GetHdc

        'get the picture 
        BitBlt(HDC2, 0, 0, rect.Width, _
          rect.Height, HDC1, rect.X, rect.Y, 13369376)

        'Clone the bitmap so we can dispose this one 
        copyRect = srcBmp.Clone()

        'Clean Up 
        srcPic.ReleaseHdc(HDC1)
        srcMem.ReleaseHdc(HDC2)
        srcPic.Dispose()
        srcMem.Dispose()
        srcMem.Dispose()
    End Function
End Module
