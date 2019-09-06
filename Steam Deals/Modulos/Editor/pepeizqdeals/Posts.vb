﻿Imports Imgur.API.Authentication.Impl
Imports Imgur.API.Endpoints.Impl
Imports Imgur.API.Models
Imports Newtonsoft.Json
Imports Windows.ApplicationModel.Core
Imports Windows.Storage
Imports Windows.System
Imports Windows.UI.Core
Imports WordPressPCL

Namespace pepeizq.Editor.pepeizqdeals
    Module Posts

        Public Async Function Enviar(titulo As String, contenido As String, categoria As Integer, etiquetas As List(Of Integer),
                                     descuento As String, precio As String, tiendaNombre As String, tiendaIcono As String,
                                     redireccion As String, imagen1 As Button, imagen2 As Button, tituloComplemento As String,
                                     analisis As JuegoAnalisis, redesSociales As Boolean, fechaTermina As String, lista As List(Of Juego), comentario As String) As Task

            Dim cliente As New WordPressClient("https://pepeizqdeals.com/wp-json/") With {
                .AuthMethod = Models.AuthMethod.JWT
            }

            Await cliente.RequestJWToken(ApplicationData.Current.LocalSettings.Values("usuarioPepeizq"), ApplicationData.Current.LocalSettings.Values("contraseñaPepeizq"))

            If Await cliente.IsValidJWToken = True Then
                Dim imagenUrl1 As String = Await SubirImagen(imagen1, "Web", cliente)

                Dim imagenUrl2 As String = String.Empty

                If Not imagen2 Is Nothing Then
                    imagenUrl2 = Await SubirImagen(imagen2, "Web2", cliente)
                End If

                Dim post As New Models.Post With {
                    .Title = New Models.Title(titulo.Trim)
                }

                If redesSociales = True Then
                    post.Status = Models.Status.Publish
                Else
                    post.Status = Models.Status.Draft
                End If

                If Not categoria = Nothing Then
                    post.Categories = New Integer() {categoria}
                End If

                Dim postString As String = JsonConvert.SerializeObject(post)

                Dim postEditor As Clases.Post = JsonConvert.DeserializeObject(Of Clases.Post)(postString)
                postEditor.FechaOriginal = DateTime.Now

                If Not etiquetas Is Nothing Then
                    If etiquetas.Count > 0 Then
                        postEditor.Etiquetas = etiquetas
                    End If
                End If

                If Not descuento = Nothing Then
                    If descuento.Trim.Length > 0 Then
                        postEditor.Descuento = descuento
                    End If
                End If

                If Not precio = Nothing Then
                    If precio.Trim.Length > 0 Then
                        postEditor.Precio = precio.Trim
                    End If
                End If

                If Not tiendaNombre = Nothing Then
                    If tiendaNombre.Trim.Length > 0 Then
                        postEditor.TiendaNombre = tiendaNombre.Trim
                    End If
                End If

                If Not tiendaIcono = Nothing Then
                    If tiendaIcono.Trim.Length > 0 Then
                        tiendaIcono = "<img src=" + ChrW(34) + tiendaIcono.Trim + ChrW(34) + " />"
                        postEditor.TiendaIcono = tiendaIcono
                    End If
                End If

                If Not redireccion = Nothing Then
                    If redireccion.Trim.Length > 0 Then
                        Dim redireccionFinal As String = redireccion.Trim
                        redireccionFinal = Referidos.Generar(redireccionFinal)
                        postEditor.Redireccion = redireccionFinal

                        postEditor.Redireccion2 = "{" + ChrW(34) + "url" + ChrW(34) + ":" + ChrW(34) + redireccionFinal + ChrW(34) +
                            "," + ChrW(34) + "target" + ChrW(34) + ":" + ChrW(34) + "_blank" + ChrW(34) + "}"
                    End If
                End If

                If Not imagenUrl1 = Nothing Then
                    If imagenUrl1.Trim.Length > 0 Then
                        postEditor.ImagenWeb1 = imagenUrl1.Trim
                    End If
                End If

                If Not imagenUrl2 = Nothing Then
                    If imagenUrl2.Trim.Length > 0 Then
                        postEditor.ImagenWeb2 = "<img src=" + ChrW(34) + imagenUrl2.Trim + ChrW(34) + " />"
                    End If
                End If

                If Not titulo = Nothing Then
                    If titulo.Trim.Length > 0 Then
                        If categoria = 3 Then
                            postEditor.SEOTitulo = titulo.Trim + " • More deals games in pepeizq's deals"
                        ElseIf categoria = 4 Then
                            postEditor.SEOTitulo = titulo.Trim + " • More bundles in pepeizq's deals"
                        ElseIf categoria = 12 Then
                            postEditor.SEOTitulo = titulo.Trim + " • More free games in pepeizq's deals"
                        ElseIf categoria = 13 Then
                            postEditor.SEOTitulo = titulo.Trim + " • Check other subscriptions in pepeizq's deals"
                        Else
                            postEditor.SEOTitulo = titulo.Trim + " • Visit pepeizq's deals to check the best deals"
                        End If
                    End If
                End If

                If Not tituloComplemento = Nothing Then
                    If tituloComplemento.Trim.Length > 0 Then
                        postEditor.TituloComplemento = tituloComplemento.Trim
                        postEditor.SEOContenido = tituloComplemento.Trim
                    End If
                End If

                If Not fechaTermina = Nothing Then
                    If fechaTermina.Trim.Length > 0 Then
                        postEditor.FechaTermina = fechaTermina.Trim
                    End If
                End If

                Dim puntuacionReview As String = String.Empty

                If Not analisis Is Nothing Then
                    Dim iconoReview As String = Nothing

                    If analisis.Porcentaje > 74 Then
                        iconoReview = "https://pepeizqdeals.com/wp-content/uploads/2018/08/review_positive.png"
                    ElseIf analisis.Porcentaje > 49 And analisis.Porcentaje < 75 Then
                        iconoReview = "https://pepeizqdeals.com/wp-content/uploads/2018/08/review_mixed.png"
                    ElseIf analisis.Porcentaje < 50 Then
                        iconoReview = "https://pepeizqdeals.com/wp-content/uploads/2018/08/review_negative.png"
                    End If

                    If Not iconoReview = Nothing Then
                        If iconoReview.Trim.Length > 0 Then
                            iconoReview = "<img src=" + ChrW(34) + iconoReview.Trim + ChrW(34) + " />"
                            postEditor.ReviewIcono = iconoReview
                        End If
                    End If

                    puntuacionReview = "Rating: " + analisis.Porcentaje + "% - Reviews: " + analisis.Cantidad

                    If Not puntuacionReview = Nothing Then
                        postEditor.ReviewPuntuacion = puntuacionReview
                    End If
                End If

                If Not contenido = Nothing Then
                    If contenido.Trim.Length > 0 Then
                        postEditor.Contenido = New Models.Content(contenido.Trim)
                    End If
                End If

                Dim resultado As Clases.Post = Nothing

                Try
                    resultado = Await cliente.CustomRequest.Create(Of Clases.Post, Clases.Post)("wp/v2/posts", postEditor)
                Catch ex As Exception

                End Try

                If Not resultado Is Nothing Then
                    Await Launcher.LaunchUriAsync(New Uri("https://pepeizqdeals.com/wp-admin/post.php?post=" + resultado.Id.ToString + "&action=edit"))

                    If redesSociales = True Then
                        Dim enlaceFinal As String = String.Empty

                        If Not resultado.Enlace = Nothing Then
                            enlaceFinal = resultado.Enlace
                        End If

                        Try
                            Await pepeizqdeals.RedesSociales.Steam.Enviar(titulo, imagenUrl1.Trim, enlaceFinal, resultado.Redireccion)
                        Catch ex As Exception
                            Notificaciones.Toast("Steam Error Post", Nothing)
                        End Try

                        Try
                            Await pepeizqdeals.RedesSociales.Twitter.Enviar(titulo, enlaceFinal, imagenUrl1.Trim, categoria)
                        Catch ex As Exception
                            Notificaciones.Toast("Twitter Error Post", Nothing)
                        End Try

                        Try
                            Await pepeizqdeals.RedesSociales.Reddit.Enviar(titulo, enlaceFinal, tituloComplemento, categoria, "/r/pepeizqdeals", Nothing, 0)
                        Catch ex As Exception
                            Notificaciones.Toast("Reddit r/pepeizqdeals Error Post", Nothing)
                        End Try

                        Try
                            Await pepeizqdeals.RedesSociales.Push.Enviar(titulo, enlaceFinal, imagenUrl1.Trim)
                        Catch ex As Exception
                            Notificaciones.Toast("Push Error Post", Nothing)
                        End Try

                        '----------------------------------------------------------------

                        If categoria = 3 Or categoria = 1218 Then
                            Dim caracteres As String = Reddit.GenerarTexto(lista)

                            If Not comentario = Nothing Then
                                If comentario.Trim.Length > 0 Then
                                    caracteres = Reddit.LimpiarComentario(comentario) + Environment.NewLine + Environment.NewLine + caracteres
                                End If
                            End If

                            If caracteres.Length < 40000 Then
                                If lista.Count = 1 Then
                                    Dim enlaceTemp As String = lista(0).Enlace + "?reddit=" + Date.Today.Year.ToString + Date.Today.DayOfYear.ToString

                                    Try
                                        Await pepeizqdeals.RedesSociales.Reddit.Enviar(titulo, enlaceTemp, tituloComplemento, categoria, "/r/GameDeals", caracteres, 0)
                                    Catch ex As Exception
                                        Notificaciones.Toast("Reddit r/GameDeals Error Post", Nothing)
                                    End Try

                                    If tiendaNombre = "Steam" Then
                                        Notificaciones.Toast("entra steamdeals", Nothing)
                                        Try
                                            Await pepeizqdeals.RedesSociales.Reddit.Enviar(titulo, enlaceTemp, tituloComplemento, categoria, "/r/steamdeals", caracteres, 0)
                                        Catch ex As Exception
                                            Notificaciones.Toast("Reddit r/steamdeals Error Post", Nothing)
                                        End Try
                                    End If
                                ElseIf lista.Count > 1 Then
                                    Try
                                        Await pepeizqdeals.RedesSociales.Reddit.Enviar(titulo, enlaceFinal, tituloComplemento, categoria, "/r/GameDeals", caracteres, 1)
                                    Catch ex As Exception
                                        Notificaciones.Toast("Reddit r/GameDeals Error Post", Nothing)
                                    End Try
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            cliente.Logout()

        End Function

        Public Async Sub Borrar()

            Await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Async Sub()
                                                                                                             Dim cliente As New WordPressClient("https://pepeizqdeals.com/wp-json/") With {
                                                                                                                 .AuthMethod = Models.AuthMethod.JWT
                                                                                                             }

                                                                                                             Try
                                                                                                                 Await cliente.RequestJWToken(ApplicationData.Current.LocalSettings.Values("usuarioPepeizq"), ApplicationData.Current.LocalSettings.Values("contraseñaPepeizq"))
                                                                                                             Catch ex As Exception

                                                                                                             End Try

                                                                                                             If Await cliente.IsValidJWToken = True Then
                                                                                                                 Dim posts As New List(Of Clases.Post)

                                                                                                                 Try
                                                                                                                     posts = Await cliente.CustomRequest.Get(Of List(Of Clases.Post))("wp/v2/posts?per_page=100")
                                                                                                                 Catch ex As Exception
                                                                                                                     Notificaciones.Toast("Error Posts Delete", Nothing)
                                                                                                                 End Try

                                                                                                                 If posts.Count > 0 Then
                                                                                                                     For Each post In posts
                                                                                                                         If Not post.FechaTermina = Nothing Then
                                                                                                                             Dim fechaTermina As Date = Date.Parse(post.FechaTermina)
                                                                                                                             Dim fechaAhora As Date = Date.Now
                                                                                                                             fechaAhora = fechaAhora.AddHours(2)

                                                                                                                             If fechaTermina < fechaAhora Then
                                                                                                                                 Try
                                                                                                                                     Await cliente.Posts.Delete(post.Id)
                                                                                                                                 Catch ex As Exception

                                                                                                                                 End Try
                                                                                                                             End If
                                                                                                                         End If
                                                                                                                     Next
                                                                                                                 End If
                                                                                                             End If
                                                                                                         End Sub)

        End Sub

        Private Async Function SubirImagen(imagen As Button, codigo As String, cliente As WordPressClient) As Task(Of String)

            Dim urlImagen As String = String.Empty

            If Not imagen Is Nothing Then
                Dim nombreFicheroImagen As String = "imagen" + codigo + Date.Now.DayOfYear.ToString + "-" + Date.Now.Hour.ToString + "-" + Date.Now.Minute.ToString + "-" + Date.Now.Millisecond.ToString + "-en.jpg"
                Dim ficheroImagen As StorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync(nombreFicheroImagen, CreationCollisionOption.ReplaceExisting)

                If Not ficheroImagen Is Nothing Then
                    Await ImagenFichero.Generar(ficheroImagen, imagen, imagen.ActualWidth, imagen.ActualHeight, 0)

                    Try
                        Dim clienteImgur As New ImgurClient("68a076ce5dadb1f", "c38ef3f6e552a36a8afc955a685b5c7e6081e202")
                        Dim endPoint As New ImageEndpoint(clienteImgur)
                        Dim imagenImgur As IImage

                        Using stream As New FileStream(ficheroImagen.Path, FileMode.Open)
                            imagenImgur = Await endPoint.UploadImageStreamAsync(stream)
                        End Using

                        urlImagen = imagenImgur.Link
                    Catch ex As Exception

                    End Try

                    If urlImagen = Nothing Then
                        Await cliente.Media.Create(ficheroImagen.Path, ficheroImagen.Name)

                        Dim mes As String = Date.Today.Month.ToString

                        If mes.Length = 1 Then
                            mes = "0" + mes
                        End If

                        urlImagen = "https://pepeizqdeals.com/wp-content/uploads/" + Date.Today.Year.ToString + "/" + mes + "/" + ficheroImagen.Name
                    End If
                End If
            End If

            Return urlImagen

        End Function

    End Module
End Namespace