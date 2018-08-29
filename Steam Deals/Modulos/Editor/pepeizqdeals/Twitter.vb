﻿Imports Microsoft.Toolkit.Services.Twitter
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.Storage.Streams

Namespace pepeizq.Editor.pepeizqdeals
    Module Twitter

        Public Async Sub Enviar(usuarioRecibido As TwitterUser, mensaje As String, enlace As String, imagen As String)

            If Not usuarioRecibido Is Nothing Then
                ApplicationData.Current.LocalSettings.Values("TwitterScreenName") = usuarioRecibido.ScreenName
            Else
                ApplicationData.Current.LocalSettings.Values("TwitterScreenName") = Nothing
            End If

            Dim servicio As New TwitterService
            servicio.Initialize("poGVvY5De5zBqQ4ceqp7jw7cj", "f8PCcuwFZxYi0r5iG6UaysgxD0NoaCT2RgYG8I41mvjghy58rc", "https://pepeizqapps.com/")

            Dim estado As Boolean = Await servicio.Provider.LoginAsync

            If estado = True Then
                Dim usuario As TwitterUser = Nothing

                If Not usuarioRecibido Is Nothing Then
                    usuario = Await servicio.Provider.GetUserAsync(usuarioRecibido.ScreenName)

                    Dim stream As FileRandomAccessStream = Nothing

                    If Not imagen = String.Empty Then
                        Dim ficheroImagen As IStorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync("imagentwitter", CreationCollisionOption.ReplaceExisting)
                        Dim descargador As New BackgroundDownloader
                        Dim descarga As DownloadOperation = descargador.CreateDownload(New Uri(imagen), ficheroImagen)
                        descarga.Priority = BackgroundTransferPriority.High
                        Await descarga.StartAsync

                        Dim ficheroDescargado As IStorageFile = descarga.ResultFile
                        If Not ficheroDescargado Is Nothing Then
                            stream = Await ficheroDescargado.OpenAsync(FileAccessMode.Read)
                        End If
                    End If

                    If stream Is Nothing Then
                        Await servicio.TweetStatusAsync(mensaje + " " + enlace)
                    Else
                        Await servicio.TweetStatusAsync(mensaje + " • " + enlace, stream.AsStream)
                    End If
                Else
                    usuario = Await servicio.GetUserAsync

                    Dim frame As Frame = Window.Current.Content
                    Dim pagina As Page = frame.Content

                    Dim imagenAvatar As ImageEx = pagina.FindName("imagenEditorTwitterpepeizqdeals")
                    imagenAvatar.Source = usuario.ProfileImageUrlHttps

                    Dim tbUsuario As TextBlock = pagina.FindName("tbEditorTwitterpepeizqdeals")
                    tbUsuario.Text = usuario.ScreenName

                    Dim helper As New LocalObjectStorageHelper
                    helper.Save("usuarioTwitter", usuario)
                End If
            End If

        End Sub

        Public Function ReemplazarTiendaTitulo(titulo As String)

            If titulo.Contains("• Amazon.es") Then
                titulo = titulo.Replace("• Amazon.es", "• @AmazonESP")
            ElseIf titulo.Contains("• Chrono") Then
                titulo = titulo.Replace("• Chrono", "• @chronodeals")
            ElseIf titulo.Contains("• Fanatical") Then
                titulo = titulo.Replace("• Fanatical", "• @Fanatical")
            ElseIf titulo.Contains("• GamersGate") Then
                titulo = titulo.Replace("• GamersGate", "• @GamersGate")
            ElseIf titulo.Contains("• GamesPlanet") Then
                titulo = titulo.Replace("• GamesPlanet", "• @GamesPlanetUK")
            ElseIf titulo.Contains("• GOG") Then
                titulo = titulo.Replace("• GOG", "• @GOGcom")
            ElseIf titulo.Contains("• Green Man Gaming") Then
                titulo = titulo.Replace("• Green Man Gaming", "• @GreenManGaming")
            ElseIf titulo.Contains("• Humble Bundle") Then
                titulo = titulo.Replace("• Humble Bundle", "• @humble")
            ElseIf titulo.Contains("• Humble Store") Then
                titulo = titulo.Replace("• Humble Store", "• @humblestore")
            ElseIf titulo.Contains("• Microsoft Store") Then
                titulo = titulo.Replace("• Microsoft Store", "• @MicrosoftStore")
            ElseIf titulo.Contains("• Sila Games") Then
                titulo = titulo.Replace("• Sila Games", "• @SilaGames")
            ElseIf titulo.Contains("• Steam") Then
                titulo = titulo.Replace("• Steam", "• @steam_games")
            ElseIf titulo.Contains("• Voidu") Then
                titulo = titulo.Replace("• Voidu", "• @voiduplay")
            ElseIf titulo.Contains("• WinGameStore") Then
                titulo = titulo.Replace("• WinGameStore", "• @wingamestore")
            End If

            Return titulo
        End Function

    End Module
End Namespace
