﻿Public Class Tienda

    Public Property NombreMostrar As String
    Public Property NombreUsar As String
    Public Property IconoApp As String
    Public Property PosicionApp As Integer
    Public Property Cupon As TiendaCupon
    Public Property EtiquetaWeb As Integer
    Public Property IconoWeb As String

    Public Sub New(ByVal nombreMostrar As String, ByVal nombreUsar As String, ByVal iconoApp As String, ByVal posicionApp As Integer,
                   ByVal cupon As TiendaCupon, ByVal etiquetaWeb As Integer, ByVal iconoWeb As String)
        Me.NombreMostrar = nombreMostrar
        Me.NombreUsar = nombreUsar
        Me.IconoApp = iconoApp
        Me.PosicionApp = posicionApp
        Me.Cupon = cupon
        Me.EtiquetaWeb = etiquetaWeb
        Me.IconoWeb = iconoWeb
    End Sub

End Class
