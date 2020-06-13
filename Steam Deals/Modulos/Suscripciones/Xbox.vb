﻿Imports Microsoft.Toolkit.Uwp.Helpers
Imports Newtonsoft.Json
Imports Steam_Deals.pepeizq.Editor.pepeizqdeals

Namespace pepeizq.Suscripciones
    Module Xbox

        Dim WithEvents Bw As New BackgroundWorker
        Dim listaIDs As New List(Of String)
        Dim listaJuegos As New List(Of JuegoSuscripcion)

        Public Async Sub BuscarJuegos(sender As Object, e As RoutedEventArgs)

            BloquearControles(False)

            Dim helper As New LocalObjectStorageHelper

            If Await helper.FileExistsAsync("listaXboxSuscripcion") Then
                listaIDs = Await helper.ReadFileAsync(Of List(Of String))("listaXboxSuscripcion")
            End If

            Bw.WorkerReportsProgress = True
            Bw.WorkerSupportsCancellation = True

            If Bw.IsBusy = False Then
                Bw.RunWorkerAsync()
            End If

        End Sub

        Private Sub Bw_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles Bw.DoWork

            Dim listaIDs2 As New List(Of String)

            Dim html_ As Task(Of String) = HttpClient(New Uri("https://reco-public.rec.mp.microsoft.com/channels/Reco/v8.0/lists/collection/XGPPMPRecentlyAdded?itemTypes=Devices&DeviceFamily=Windows.Desktop&market=US&language=EN&count=200"))
            Dim html As String = html_.Result

            If Not html = Nothing Then
                Dim juegos As MicrosoftStoreBBDDIDs = JsonConvert.DeserializeObject(Of MicrosoftStoreBBDDIDs)(html)

                If Not juegos Is Nothing Then
                    For Each juego In juegos.Juegos
                        Dim añadir As Boolean = True

                        If Not listaIDs Is Nothing Then
                            For Each id In listaIDs
                                If id = juego.ID Then
                                    añadir = False
                                End If
                            Next
                        End If

                        If añadir = True Then
                            listaIDs.Add(juego.ID)
                            listaIDs2.Add(juego.ID)
                        End If
                    Next
                End If
            End If

            If listaIDs2.Count > 0 Then
                Dim ids As String = String.Empty

                For Each id In listaIDs2
                    ids = ids + id + ","
                Next

                If ids.Length > 0 Then
                    ids = ids.Remove(ids.Length - 1)

                    Dim htmlJuego_ As Task(Of String) = HttpClient(New Uri("https://displaycatalog.mp.microsoft.com/v7.0/products?bigIds=" + ids + "&market=US&languages=en-us&MS-CV=DGU1mcuYo0WMMp+F.1"))
                    Dim htmlJuego As String = htmlJuego_.Result

                    If Not htmlJuego = Nothing Then
                        Dim juegos As MicrosoftStoreBBDDDetalles = JsonConvert.DeserializeObject(Of MicrosoftStoreBBDDDetalles)(htmlJuego)

                        For Each juego In juegos.Juegos
                            Dim imagenLista As String = String.Empty

                            For Each imagen In juego.Detalles(0).Imagenes
                                If imagen.Proposito = "Poster" Then
                                    imagenLista = imagen.Enlace

                                    If Not imagenLista.Contains("http:") Then
                                        imagenLista = "http:" + imagenLista
                                    End If
                                End If
                            Next

                            If Not imagenLista = Nothing Then
                                If Not juego.Propiedades.Detalles Is Nothing Then
                                    For Each detalle In juego.Propiedades.Detalles
                                        If Not detalle.Plataformas Is Nothing Then
                                            For Each plataforma In detalle.Plataformas
                                                If plataforma = "Desktop" Then
                                                    Dim añadir As Boolean = True

                                                    For Each juegolista In listaJuegos
                                                        If juegolista.Titulo = juego.Detalles(0).Titulo.Trim Then
                                                            añadir = False
                                                        End If
                                                    Next

                                                    If añadir = True Then
                                                        Dim titulo As String = juego.Detalles(0).Titulo.Trim
                                                        titulo = LimpiarTitulo(titulo)

                                                        listaJuegos.Add(New JuegoSuscripcion(titulo, imagenLista, Nothing, Referidos.Generar("https://www.microsoft.com/store/apps/" + juego.ID), Nothing))
                                                    End If
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                            End If
                        Next
                    End If
                End If
            End If

        End Sub

        Private Async Sub Bw_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles Bw.RunWorkerCompleted

            Dim helper As New LocalObjectStorageHelper
            Await helper.SaveFileAsync(Of List(Of String))("listaXboxSuscripcion", listaIDs)

            Html.Generar("Microsoft Store", Referidos.Generar("https://www.microsoft.com/en-us/p/xbox-game-pass-pc-games/cfq7ttc0kgq8"), Nothing, listaJuegos, True)

            BloquearControles(True)

        End Sub

        Public Function LimpiarTitulo(titulo As String)

            titulo = titulo.Replace("(PC)", Nothing)
            titulo = titulo.Replace("for Windows 10", Nothing)
            titulo = titulo.Replace("– Windows 10", Nothing)
            titulo = titulo.Trim

            Return titulo
        End Function

        Public Class MicrosoftStoreBBDDIDs

            <JsonProperty("Items")>
            Public Juegos As List(Of MicrosoftStoreBBDDIDsJuego)

        End Class

        Public Class MicrosoftStoreBBDDIDsJuego

            <JsonProperty("Id")>
            Public ID As String

        End Class

        '-------------------------

        Public Class MicrosoftStoreBBDDDetalles

            <JsonProperty("Products")>
            Public Juegos As List(Of MicrosoftStoreBBDDDetallesJuego)

        End Class

        Public Class MicrosoftStoreBBDDDetallesJuego

            <JsonProperty("LocalizedProperties")>
            Public Detalles As List(Of MicrosoftStoreBBDDDetallesJuego2)

            <JsonProperty("Properties")>
            Public Propiedades As MicrosoftStoreBBDDDetallesPropiedades

            <JsonProperty("ProductId")>
            Public ID As String

            <JsonProperty("DisplaySkuAvailabilities")>
            Public Propiedades2 As List(Of MicrosoftStoreBBDDDetallesPropiedades2)

        End Class

        Public Class MicrosoftStoreBBDDDetallesJuego2

            <JsonProperty("ProductTitle")>
            Public Titulo As String

            <JsonProperty("Images")>
            Public Imagenes As List(Of MicrosoftStoreBBDDDetallesJuego2Imagen)

        End Class

        Public Class MicrosoftStoreBBDDDetallesJuego2Imagen

            <JsonProperty("ImagePurpose")>
            Public Proposito As String

            <JsonProperty("Uri")>
            Public Enlace As String

        End Class

        Public Class MicrosoftStoreBBDDDetallesPropiedades

            <JsonProperty("Attributes")>
            Public Detalles As List(Of MicrosoftStoreBBDDDetallesPropiedadesDetalles)

        End Class

        Public Class MicrosoftStoreBBDDDetallesPropiedadesDetalles

            <JsonProperty("ApplicablePlatforms")>
            Public Plataformas As List(Of String)

        End Class

        Public Class MicrosoftStoreBBDDDetallesPropiedades2

            <JsonProperty("Availabilities")>
            Public Disponible As List(Of MicrosoftStoreBBDDDetallesPropiedades2Disponibilidad)

        End Class

        Public Class MicrosoftStoreBBDDDetallesPropiedades2Disponibilidad

            <JsonProperty("OrderManagementData")>
            Public Datos As MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatos

        End Class

        Public Class MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatos

            <JsonProperty("Price")>
            Public Precio As MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatosPrecio

        End Class

        Public Class MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatosPrecio

            <JsonProperty("ListPrice")>
            Public PrecioRebajado As String

            <JsonProperty("MSRP")>
            Public PrecioBase As String

        End Class

    End Module
End Namespace

