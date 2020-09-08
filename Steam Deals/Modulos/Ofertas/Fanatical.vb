﻿Imports System.Net
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Newtonsoft.Json
Imports Windows.Storage

Namespace pepeizq.Ofertas
    Module Fanatical

        Dim WithEvents Bw As New BackgroundWorker
        Dim listaJuegos As New List(Of Oferta)
        Dim listaAnalisis As New List(Of OfertaAnalisis)
        Dim Tienda As Tienda = Nothing
        Dim cuponPorcentaje As String = String.Empty

        Public Async Sub BuscarOfertas(tienda_ As Tienda)

            Tienda = tienda_

            Dim helper As New LocalObjectStorageHelper

            If Await helper.FileExistsAsync("listaAnalisis") Then
                listaAnalisis = Await helper.ReadFileAsync(Of List(Of OfertaAnalisis))("listaAnalisis")
            End If

            Dim listaCupones As New List(Of TiendaCupon)

            If Await helper.FileExistsAsync("cupones") = True Then
                listaCupones = Await helper.ReadFileAsync(Of List(Of TiendaCupon))("cupones")
            End If

            If listaCupones.Count > 0 Then
                For Each cupon In listaCupones
                    If Tienda.NombreUsar = cupon.TiendaNombreUsar Then
                        If Not cupon.Porcentaje = Nothing Then
                            If cupon.Porcentaje > 0 Then
                                cuponPorcentaje = cupon.Porcentaje
                                cuponPorcentaje = cuponPorcentaje.Replace("%", Nothing)
                                cuponPorcentaje = cuponPorcentaje.Trim

                                If cuponPorcentaje.Length = 1 Then
                                    cuponPorcentaje = "0,0" + cuponPorcentaje
                                Else
                                    cuponPorcentaje = "0," + cuponPorcentaje
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim spProgreso As StackPanel = pagina.FindName("spTiendaProgreso" + Tienda.NombreUsar)
            spProgreso.Visibility = Visibility.Visible

            listaJuegos.Clear()

            Bw.WorkerReportsProgress = True
            Bw.WorkerSupportsCancellation = True

            If Bw.IsBusy = False Then
                Bw.RunWorkerAsync()
            End If

        End Sub

        Private Sub Bw_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles Bw.DoWork

            Dim html_ As Task(Of String) = Decompiladores.HttpClient(New Uri("https://feed.fanatical.com/feed"))
            Dim html As String = "[" + html_.Result + "]"
            html = html.Replace("{" + ChrW(34) + "features", ",{" + ChrW(34) + "features")

            Dim int3 As Integer = html.IndexOf(",")
            html = html.Remove(int3, 1)

            Dim juegosFanatical As List(Of FanaticalJuego) = JsonConvert.DeserializeObject(Of List(Of FanaticalJuego))(html)

            For Each juegoFanatical In juegosFanatical
                Dim titulo As String = juegoFanatical.Titulo
                titulo = WebUtility.HtmlDecode(titulo)
                titulo = Text.RegularExpressions.Regex.Unescape(titulo)

                Dim enlace As String = juegoFanatical.Enlace

                Dim imagenPequeña As String = juegoFanatical.Imagen
                Dim imagenes As New OfertaImagenes(imagenPequeña, Nothing)

                Dim descuento As String = juegoFanatical.Descuento

                If Not descuento = Nothing Then
                    If descuento.Contains(".") Then
                        Dim intDescuento As Integer = descuento.IndexOf(".")
                        descuento = descuento.Remove(intDescuento, descuento.Length - intDescuento)
                    End If

                    If descuento.Length = 1 Then
                        descuento = "0" + descuento
                    End If

                    descuento = descuento.Trim + "%"
                End If

                Dim precio As String = juegoFanatical.PrecioRebajado.EUR

                If precio = Nothing Then
                    If Not juegoFanatical.PrecioBase.EUR = Nothing Then
                        precio = Calculadora.GenerarPrecioRebajado(juegoFanatical.PrecioBase.EUR, descuento)
                    End If
                End If

                If Not precio = Nothing Then
                    precio = precio + " €"

                    If Not cuponPorcentaje = Nothing Then
                        precio = precio.Replace(",", ".")
                        precio = precio.Replace("€", Nothing)
                        precio = precio.Trim

                        Dim dprecio As Double = Double.Parse(precio, Globalization.CultureInfo.InvariantCulture) - (Double.Parse(precio, Globalization.CultureInfo.InvariantCulture) * cuponPorcentaje)
                        precio = Math.Round(dprecio, 2).ToString + " €"
                        descuento = Calculadora.GenerarDescuento(juegoFanatical.PrecioBase.EUR, precio)
                    End If
                End If

                Dim drm As String = Nothing

                If Not juegoFanatical.DRM Is Nothing Then
                    If juegoFanatical.DRM.Count > 0 Then
                        drm = juegoFanatical.DRM(0)
                    End If
                End If

                Dim windows As Boolean = False
                Dim mac As Boolean = False
                Dim linux As Boolean = False

                For Each sistema In juegoFanatical.Sistemas
                    If sistema = "windows" Then
                        windows = True
                    End If

                    If sistema = "mac" Then
                        mac = True
                    End If

                    If sistema = "linux" Then
                        linux = True
                    End If
                Next

                Dim sistemas As New OfertaSistemas(windows, mac, linux)

                Dim ana As OfertaAnalisis = Analisis.BuscarJuego(titulo, listaAnalisis, juegoFanatical.SteamID)

                Dim fechaTermina As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                Try
                    fechaTermina = fechaTermina.AddSeconds(Convert.ToDouble(juegoFanatical.Fecha))
                    fechaTermina = fechaTermina.ToLocalTime
                Catch ex As Exception

                End Try

                Dim desarrolladores As New OfertaDesarrolladores(juegoFanatical.Publishers, Nothing)

                Dim tipo As String = juegoFanatical.Tipo

                Dim juego As New Oferta(titulo, descuento, precio, enlace, imagenes, drm, Tienda, Nothing, tipo, DateTime.Today, fechaTermina, ana, sistemas, desarrolladores)

                Dim añadir As Boolean = True
                Dim k As Integer = 0
                While k < listaJuegos.Count
                    If listaJuegos(k).Enlace = juego.Enlace Then
                        añadir = False
                    End If
                    k += 1
                End While

                If añadir = True Then
                    If Not juegoFanatical.Regiones Is Nothing Then
                        If juegoFanatical.Regiones.Count > 0 Then
                            añadir = False
                            For Each region In juegoFanatical.Regiones
                                If region = "ES" Then
                                    añadir = True
                                End If
                            Next
                        End If
                    End If

                    If juego.Descuento = Nothing Then
                        juego.Descuento = "00%"
                    Else
                        If juego.Descuento = "null%" Then
                            juego.Descuento = "00%"
                        ElseIf juego.Descuento.Contains("-") Then
                            juego.Descuento = "00%"
                        End If
                    End If

                    If añadir = True Then
                        juego.Precio = Ordenar.PrecioPreparar(juego.Precio)

                        listaJuegos.Add(juego)
                    End If
                End If
            Next

        End Sub

        Private Sub Bw_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles Bw.ProgressChanged

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim pb As ProgressBar = pagina.FindName("pbTiendaProgreso" + Tienda.NombreUsar)
            pb.Value = e.ProgressPercentage

            Dim tb As TextBlock = pagina.FindName("tbTiendaProgreso" + Tienda.NombreUsar)
            tb.Text = e.ProgressPercentage.ToString + "%"

        End Sub

        Private Async Sub Bw_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles Bw.RunWorkerCompleted

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim spProgreso As StackPanel = pagina.FindName("spTiendaProgreso" + Tienda.NombreUsar)
            spProgreso.Visibility = Visibility.Collapsed

            Dim helper As New LocalObjectStorageHelper
            Await helper.SaveFileAsync(Of List(Of Oferta))("listaOfertas" + Tienda.NombreUsar, listaJuegos)

            Ordenar.Ofertas(Tienda, True, False)

        End Sub

    End Module

    Public Class FanaticalJuego

        <JsonProperty("title")>
        Public Titulo As String

        <JsonProperty("sku")>
        Public ID As String

        <JsonProperty("publishers")>
        Public Publishers As List(Of String)

        <JsonProperty("developers")>
        Public Desarrolladores As List(Of String)

        <JsonProperty("operating_systems")>
        Public Sistemas As List(Of String)

        <JsonProperty("drm")>
        Public DRM As List(Of String)

        <JsonProperty("image")>
        Public Imagen As String

        <JsonProperty("url")>
        Public Enlace As String

        <JsonProperty("discount_percent")>
        Public Descuento As String

        <JsonProperty("expiry")>
        Public Fecha As String

        <JsonProperty("steam_app_id")>
        Public SteamID As String

        <JsonProperty("current_price")>
        Public PrecioRebajado As FanaticalJuegoPrecio

        <JsonProperty("regular_price")>
        Public PrecioBase As FanaticalJuegoPrecio

        <JsonProperty("regions")>
        Public Regiones As List(Of String)

        <JsonProperty("bundle_games")>
        Public Bundle As FanaticalJuegoBundle

        <JsonProperty("type")>
        Public Tipo As String

    End Class

    Public Class FanaticalJuegoPrecio

        <JsonProperty("USD")>
        Public USD As String

        <JsonProperty("GBP")>
        Public GBP As String

        <JsonProperty("EUR")>
        Public EUR As String

    End Class

    Public Class FanaticalJuegoBundle

        <JsonProperty("1")>
        Public Tier1 As FanaticalJuegoBundleTier

        <JsonProperty("2")>
        Public Tier2 As FanaticalJuegoBundleTier

        <JsonProperty("3")>
        Public Tier3 As FanaticalJuegoBundleTier

    End Class

    Public Class FanaticalJuegoBundleTier

        <JsonProperty("items")>
        Public Juegos As List(Of FanaticalJuegoBundleJuego)

    End Class

    Public Class FanaticalJuegoBundleJuego

        <JsonProperty("steam_id")>
        Public IDSteam As String

    End Class
End Namespace

