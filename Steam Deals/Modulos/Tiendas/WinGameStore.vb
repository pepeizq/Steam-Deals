﻿Imports Microsoft.Toolkit.Uwp.Helpers
Imports Newtonsoft.Json

Namespace pepeizq.Tiendas
    Module WinGameStore

        Dim WithEvents Bw As New BackgroundWorker
        Dim listaJuegos As New List(Of Juego)
        Dim listaAnalisis As New List(Of JuegoAnalisis)
        Dim listaDesarrolladores As New List(Of WinGameStoreDesarrolladores)
        Dim Tienda As Tienda = Nothing
        Dim dolar As String = String.Empty

        Public Async Sub BuscarOfertas(tienda_ As Tienda)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbDolar As TextBlock = pagina.FindName("tbDivisasDolar")
            dolar = tbDolar.Text

            Tienda = tienda_

            Dim helper As New LocalObjectStorageHelper

            If Await helper.FileExistsAsync("listaAnalisis") Then
                listaAnalisis = Await helper.ReadFileAsync(Of List(Of JuegoAnalisis))("listaAnalisis")
            End If

            If Await helper.FileExistsAsync("listaDesarrolladoresWinGameStore") Then
                listaDesarrolladores = Await helper.ReadFileAsync(Of List(Of WinGameStoreDesarrolladores))("listaDesarrolladoresWinGameStore")
            Else
                listaDesarrolladores = New List(Of WinGameStoreDesarrolladores)
            End If

            Dim tb As TextBlock = pagina.FindName("tbOfertasProgreso")
            tb.Text = "0%"

            listaJuegos.Clear()

            Bw.WorkerReportsProgress = True
            Bw.WorkerSupportsCancellation = True

            If Bw.IsBusy = False Then
                Bw.RunWorkerAsync()
            End If

        End Sub

        Private Sub Bw_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles Bw.DoWork

            Dim html_ As Task(Of String) = HttpClient(New Uri("https://www.macgamestore.com/affiliate/feeds/p_C1B2A3.json"))
            Dim html As String = html_.Result

            If Not html = Nothing Then
                Dim listaJuegosWGS As List(Of WinGameStoreJuego) = JsonConvert.DeserializeObject(Of List(Of WinGameStoreJuego))(html)

                If Not listaJuegosWGS Is Nothing Then
                    If listaJuegosWGS.Count > 0 Then
                        For Each juegoWGS In listaJuegosWGS
                            If Not juegoWGS.PrecioRebajado = "0" Then
                                Dim titulo As String = juegoWGS.Titulo.Trim
                                titulo = Text.RegularExpressions.Regex.Unescape(titulo)

                                Dim enlace As String = juegoWGS.Enlace

                                If Not enlace = String.Empty Then
                                    If enlace.Contains("?") Then
                                        Dim int As Integer = enlace.IndexOf("?")
                                        enlace = enlace.Remove(int, enlace.Length - int)
                                    End If

                                    Dim precio As String = "$" + juegoWGS.PrecioRebajado.Trim

                                    If Not precio.Contains(".") Then
                                        precio = precio + ".00"
                                    End If

                                    Dim imagenPequeña As String = juegoWGS.Imagen

                                    Dim imagenes As New JuegoImagenes(imagenPequeña, Nothing)

                                    Dim descuento As String = Calculadora.GenerarDescuento(juegoWGS.PrecioBase.Trim, precio)

                                    Dim drm As String = juegoWGS.DRM

                                    Dim windows As Boolean = False

                                    If juegoWGS.Sistemas.Contains("windows") Then
                                        windows = True
                                    End If

                                    Dim mac As Boolean = False

                                    If juegoWGS.Sistemas.Contains("mac") Then
                                        mac = True
                                    End If

                                    Dim linux As Boolean = False

                                    If juegoWGS.Sistemas.Contains("linux") Then
                                        linux = True
                                    End If

                                    Dim sistemas As New JuegoSistemas(windows, mac, linux)

                                    Dim ana As JuegoAnalisis = Analisis.BuscarJuego(titulo, listaAnalisis, juegoWGS.SteamID)

                                    Dim juego As New Juego(titulo, descuento, precio, enlace, imagenes, drm, Tienda, Nothing, Nothing, DateTime.Today, Nothing, ana, sistemas, Nothing)

                                    Dim tituloBool As Boolean = False
                                    Dim k As Integer = 0
                                    While k < listaJuegos.Count
                                        If listaJuegos(k).Titulo = juego.Titulo Then
                                            tituloBool = True
                                        End If
                                        k += 1
                                    End While

                                    If juego.Descuento = Nothing Then
                                        tituloBool = True
                                    Else
                                        If juego.Descuento = "00%" Then
                                            tituloBool = True
                                        End If
                                    End If

                                    If tituloBool = False Then
                                        For Each desarrollador In listaDesarrolladores
                                            If desarrollador.ID = juegoWGS.ID Then
                                                juego.Desarrolladores = New JuegoDesarrolladores(New List(Of String) From {desarrollador.Desarrollador}, Nothing)
                                                Exit For
                                            End If
                                        Next

                                        juego.Precio = CambioMoneda(juego.Precio, dolar)

                                        listaJuegos.Add(juego)
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            End If

            Dim i As Integer = 0
            For Each juego In listaJuegos
                If juego.Desarrolladores Is Nothing Then
                    Dim htmlJuego_ As Task(Of String) = HttpClient(New Uri(juego.Enlace))
                    Dim htmlJuego As String = htmlJuego_.Result

                    If Not htmlJuego = Nothing Then
                        If htmlJuego.Contains("<label>Publisher</label>") Then
                            Dim temp, temp2, temp3 As String
                            Dim int, int2, int3 As Integer

                            int = htmlJuego.IndexOf("<label>Publisher</label>")
                            temp = htmlJuego.Remove(0, int + 5)

                            int2 = temp.IndexOf("</a>")
                            temp2 = temp.Remove(int2, temp.Length - int2)

                            int3 = temp2.LastIndexOf(">")
                            temp3 = temp2.Remove(0, int3 + 1)

                            juego.Desarrolladores = New JuegoDesarrolladores(New List(Of String) From {temp3.Trim}, Nothing)

                            Dim id As String = juego.Enlace

                            If id.Contains("/product/") Then
                                Dim int4 As Integer = id.IndexOf("/product/")
                                id = id.Remove(0, int4 + 9)

                                int4 = id.IndexOf("/")
                                id = id.Remove(int4, id.Length - int4)
                            End If

                            If id.Contains("/product-goto/") Then
                                Dim int4 As Integer = id.IndexOf("/product-goto/")
                                id = id.Remove(0, int4 + 14)

                                int4 = id.IndexOf("/")
                                id = id.Remove(int4, id.Length - int4)
                            End If

                            listaDesarrolladores.Add(New WinGameStoreDesarrolladores(id, temp3.Trim))
                        End If
                    End If
                End If

                Dim porcentaje As Integer = CInt((100 / listaJuegos.Count) * i)
                Bw.ReportProgress(porcentaje)
                i += 1
            Next

        End Sub

        Private Sub Bw_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles Bw.ProgressChanged

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tb As TextBlock = pagina.FindName("tbOfertasProgreso")
            tb.Text = e.ProgressPercentage.ToString + "%"

        End Sub

        Private Async Sub Bw_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles Bw.RunWorkerCompleted

            Dim helper As New LocalObjectStorageHelper
            Await helper.SaveFileAsync(Of List(Of Juego))("listaOfertas" + Tienda.NombreUsar, listaJuegos)
            Await helper.SaveFileAsync(Of List(Of WinGameStoreDesarrolladores))("listaDesarrolladoresWinGameStore", listaDesarrolladores)

            Ordenar.Ofertas(Tienda.NombreUsar, True, False)

        End Sub

    End Module

    Public Class WinGameStoreJuego

        <JsonProperty("title")>
        Public Titulo As String

        <JsonProperty("url")>
        Public Enlace As String

        <JsonProperty("current_price")>
        Public PrecioRebajado As String

        <JsonProperty("retail_price")>
        Public PrecioBase As String

        <JsonProperty("pid")>
        Public ID As String

        <JsonProperty("platforms")>
        Public Sistemas As List(Of String)

        <JsonProperty("drm")>
        Public DRM As String

        <JsonProperty("drmid")>
        Public SteamID As String

        <JsonProperty("badge")>
        Public Imagen As String

    End Class

    Public Class WinGameStoreDesarrolladores

        Public Property ID As String
        Public Property Desarrollador As String

        Public Sub New(ByVal id As String, ByVal desarrollador As String)
            Me.ID = id
            Me.Desarrollador = desarrollador
        End Sub

    End Class
End Namespace

