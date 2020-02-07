﻿Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Steam_Deals.pepeizq.Editor.pepeizqdeals
Imports Windows.UI

Namespace pepeizq.Suscripciones
    Module Xbox

        Dim WithEvents Bw As New BackgroundWorker
        Dim listaIDs As New List(Of String)
        Dim listaJuegos As New List(Of JuegoImagen)

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
                                                        listaJuegos.Add(New JuegoImagen(juego.Detalles(0).Titulo.Trim, imagenLista, juego.ID))
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

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbTitulo As TextBox = pagina.FindName("tbEditorTitulopepeizqdealsSubscriptions")
            Dim tbJuegos As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsJuegos")
            Dim tbIDs As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsIDs")

            Dim gvImagen As GridView = pagina.FindName("gvEditorpepeizqdealsImagenEntradaSubscriptions2")
            gvImagen.Items.Clear()

            If Not listaJuegos Is Nothing Then
                If listaJuegos.Count > 0 Then
                    gvImagen.Visibility = Visibility.Visible

                    Dim i As Integer = 0
                    For Each juego In listaJuegos

                        If i = 0 Then
                            tbTitulo.Text = tbTitulo.Text + juego.Titulo.Trim
                            tbJuegos.Text = juego.Titulo.Trim
                            tbIDs.Text = juego.Imagen
                        ElseIf i = (listaJuegos.Count - 1) Then
                            tbTitulo.Text = tbTitulo.Text + " and " + juego.Titulo.Trim
                            tbJuegos.Text = tbJuegos.Text + " and " + juego.Titulo.Trim
                        Else
                            tbTitulo.Text = tbTitulo.Text + ", " + juego.Titulo.Trim
                            tbJuegos.Text = tbJuegos.Text + ", " + juego.Titulo.Trim
                            tbIDs.Text = tbIDs.Text + "," + juego.Imagen
                        End If

                        Dim margin As Integer = 0

                        If listaJuegos.Count = 1 Then
                            margin = 8
                        ElseIf listaJuegos.Count = 2 Then
                            margin = 8
                        ElseIf listaJuegos.Count = 3 Then
                            margin = 8
                        Else
                            margin = 5
                        End If

                        Dim panel As New DropShadowPanel With {
                            .BlurRadius = 15,
                            .ShadowOpacity = 0.9,
                            .Color = Colors.Black,
                            .Margin = New Thickness(margin, margin, margin, margin)
                        }

                        Dim colorFondo2 As New SolidColorBrush With {
                            .Color = "#004e7a".ToColor
                        }

                        Dim gridContenido As New Grid With {
                            .Background = colorFondo2
                        }

                        Dim imagenJuego As New ImageEx With {
                            .Stretch = Stretch.Uniform,
                            .IsCacheEnabled = True,
                            .Source = juego.Imagen
                        }

                        If listaJuegos.Count = 1 Then
                            imagenJuego.MaxHeight = 320
                        ElseIf listaJuegos.Count = 2 Then
                            imagenJuego.MaxHeight = 320
                        ElseIf listaJuegos.Count = 3 Then
                            imagenJuego.MaxHeight = 320
                        Else
                            imagenJuego.MaxHeight = 175
                        End If

                        gridContenido.Children.Add(imagenJuego)
                        panel.Content = gridContenido

                        gvImagen.Items.Add(panel)

                        i += 1
                    Next

                    Dim cosas As Clases.Suscripciones = tbTitulo.Tag

                    Dim ancho As String = String.Empty

                    If listaJuegos.Count = 2 Then
                        ancho = " width=" + ChrW(34) + "1/2" + ChrW(34)
                    ElseIf listaJuegos.Count = 3 Then
                        ancho = " width=" + ChrW(34) + "1/3" + ChrW(34)
                    ElseIf listaJuegos.Count > 3 Then
                        ancho = " width=" + ChrW(34) + "1/4" + ChrW(34)
                    End If

                    Dim html As String = String.Empty

                    html = "[vc_row width=" + ChrW(34) + "full" + ChrW(34) + " bg_type=" + ChrW(34) + "bg_color" + ChrW(34) + " bg_color_value=" + ChrW(34) + "#004E7a" + ChrW(34) + "][vc_column][us_btn label=" + ChrW(34) + "Buy Subscription" + ChrW(34) + " link=" + ChrW(34) + "url:http%3A%2F%2Fmicrosoft.msafflnk.net%2FEYkmK||target: %20_blank|" + ChrW(34) + " style=" + ChrW(34) + "4" + ChrW(34) + " align=" + ChrW(34) + "center" + ChrW(34) + "][/vc_column][/vc_row]"

                    Dim j As Integer = 0
                    While j < listaJuegos.Count
                        If j = 0 Or j = 4 Or j = 8 Then
                            html = html + "[vc_row el_class=" + ChrW(34) + "tope" + ChrW(34) + "]"
                        End If

                        html = html + "[vc_column " + ancho + "][vc_column_text]"
                        html = html + "<a href=" + ChrW(34) + Referidos.Generar("https://www.microsoft.com/store/apps/" + listaJuegos(j).ID) + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + "><img style=" + ChrW(34) + "display: block; margin-left: auto; margin-right: auto;" + ChrW(34) + " src=" + ChrW(34) + listaJuegos(j).Imagen + ChrW(34) + "></a><div style=" + ChrW(34) + "text-align: center; margin-top: 5px; font-size: 17px;" + ChrW(34) + "><a style=" + ChrW(34) + "color: white;" + ChrW(34) + " href=" + ChrW(34) + Referidos.Generar("https://www.microsoft.com/store/apps/" + listaJuegos(j).ID) + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + ">" + listaJuegos(j).Titulo + "</a></div>"
                        html = html + "[/vc_column_text][/vc_column]"

                        If j = listaJuegos.Count - 1 Or j = 3 Or j = 7 Then
                            html = html + "[/vc_row]"
                        End If

                        j += 1
                    End While

                    cosas.Html = html
                End If
            End If

            BloquearControles(True)

        End Sub


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

        Public Class JuegoImagen

            Public Property Titulo As String
            Public Property Imagen As String
            Public Property ID As String

            Public Sub New(ByVal titulo As String, ByVal imagen As String, ByVal id As String)
                Me.Titulo = titulo
                Me.Imagen = imagen
                Me.ID = id
            End Sub

        End Class

    End Module
End Namespace
