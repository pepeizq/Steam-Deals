﻿Imports Microsoft.Toolkit.Uwp

Module WinGameStore

    Dim WithEvents bw As New BackgroundWorker
    Dim listaJuegos As New List(Of Juego)

    Public Sub GenerarOfertas()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim lv As ListView = pagina.FindName("listadoWinGameStore")
        lv.IsEnabled = False
        lv.Items.Clear()

        Dim cbOrdenar As ComboBox = pagina.FindName("cbOrdenarWinGameStore")
        cbOrdenar.IsEnabled = False

        Dim cbDRM As ComboBox = pagina.FindName("cbDRMWinGameStore")
        cbDRM.IsEnabled = False

        Dim gridProgreso As Grid = pagina.FindName("gridProgresoWinGameStore")
        gridProgreso.Visibility = Visibility.Visible

        bw.WorkerReportsProgress = True
        bw.WorkerSupportsCancellation = True

        If bw.IsBusy = False Then
            bw.RunWorkerAsync()
        End If

    End Sub

    Private Sub bw_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bw.DoWork

        Dim html_ As Task(Of String) = HttpClient(New Uri("https://www.macgamestore.com/api.php?p=games&s=wgs"))
        Dim html As String = html_.Result

        Dim i As Integer = 0
        While i < 5000
            If Not html = Nothing Then
                If html.Contains("{" + ChrW(34) + "ID") Then
                    Dim temp, temp2 As String
                    Dim int, int2 As Integer

                    int = html.IndexOf("{" + ChrW(34) + "ID")
                    temp = html.Remove(0, int + 4)

                    html = temp

                    int2 = temp.IndexOf("}")
                    temp2 = temp.Remove(int2, temp.Length - int2)

                    Dim temp3, temp4 As String
                    Dim int3, int4 As Integer

                    int3 = temp2.IndexOf("Title" + ChrW(34) + ":" + ChrW(34))
                    temp3 = temp2.Remove(0, int3 + 8)

                    int4 = temp3.IndexOf(ChrW(34))
                    temp4 = temp3.Remove(int4, temp3.Length - int4)

                    temp4 = temp4.Replace("\u00b2", "²")
                    temp4 = temp4.Replace("\u00ae", "®")
                    temp4 = temp4.Replace("\u2013", "–")
                    temp4 = temp4.Replace("\u2122", "™")

                    Dim titulo As String = temp4.Trim

                    Dim temp5, temp6 As String
                    Dim int5, int6 As Integer

                    int5 = temp2.IndexOf("http:\/\/www.wingamestore")
                    temp5 = temp2.Remove(0, int5)

                    int6 = temp5.IndexOf(ChrW(34))
                    temp6 = temp5.Remove(int6, temp5.Length - int6)

                    temp6 = temp6.Replace("\", Nothing)

                    Dim enlace As String = temp6.Trim

                    Dim imagen As String = Nothing

                    Dim temp7 As String
                    Dim int7 As Integer

                    int7 = temp2.IndexOf("Sale" + ChrW(34) + ":")
                    temp7 = temp2.Remove(0, int7 + 6)

                    If Not temp7 = "0" Then
                        Dim precio As String = "$" + temp7.Trim

                        If Not precio.Contains(".") Then
                            precio = precio + ".00"
                        End If

                        Dim temp9, temp10 As String
                        Dim int9, int10 As Integer

                        int9 = temp2.IndexOf("Price" + ChrW(34) + ":")
                        temp9 = temp2.Remove(0, int9 + 7)

                        int10 = temp9.IndexOf(",")
                        temp10 = temp9.Remove(int10, temp9.Length - int10)

                        Dim descuento As String = Calculadora.GenerarDescuento(temp10.Trim, precio)

                        Dim drm As String = Nothing

                        Dim juego As New Juego(titulo, enlace, imagen, precio, Nothing, descuento, drm, False, False, False, "WinGameStore")

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
                        End If

                        If tituloBool = False Then
                            listaJuegos.Add(juego)
                        End If
                    End If
                End If
            End If
            i += 1
        End While

        i = 0
        For Each juego In listaJuegos

            Dim htmlJuego_ As Task(Of String) = HttpClient(New Uri(juego.Enlace))
            Dim htmlJuego As String = htmlJuego_.Result

            If Not htmlJuego = Nothing Then
                If htmlJuego.Contains("<meta property=" + ChrW(34) + "og:image") Then
                    Dim temp, temp2, temp3 As String
                    Dim int, int2, int3 As Integer

                    int = htmlJuego.IndexOf("<meta property=" + ChrW(34) + "og:image")
                    temp = htmlJuego.Remove(0, int + 5)

                    int2 = temp.IndexOf("content=")
                    temp2 = temp.Remove(0, int2 + 9)

                    int3 = temp2.IndexOf(ChrW(34))
                    temp3 = temp2.Remove(int3, temp2.Length - int3)

                    juego.Imagen = temp3.Trim
                End If

                If htmlJuego.Contains("<div class=" + ChrW(34) + "image-wrap220") Then
                    Dim temp, temp2, temp3 As String
                    Dim int, int2, int3 As Integer

                    int = htmlJuego.IndexOf("<div class=" + ChrW(34) + "image-wrap220")
                    temp = htmlJuego.Remove(0, int + 5)

                    int2 = temp.IndexOf("<img src=")
                    temp2 = temp.Remove(0, int2 + 10)

                    int3 = temp2.IndexOf(ChrW(34))
                    temp3 = temp2.Remove(int3, temp2.Length - int3)

                    juego.Imagen = "http://www.wingamestore.com" + temp3.Trim
                End If

                If htmlJuego.Contains("<label>DRM</label><b>Steam</b>") Then
                    juego.DRM = "steam"
                ElseIf htmlJuego.Contains("<label>DRM</label><b>Steam & DRM Free</b>") Then
                    juego.DRM = "steam"
                End If

                If juego.Enlace.Contains("3861/XCOM-Enemy-Within") Then
                    juego.DRM = "steam"
                End If
            End If

            juego.Enlace = "http://click.linksynergy.com/fs-bin/click?id=15NET1Ktcr4&subid=&offerid=283896.1&type=10&tmpid=11753&RD_PARM1=" + juego.Enlace

            Dim porcentaje As Integer = CInt((100 / listaJuegos.Count) * i)
            bw.ReportProgress(porcentaje)
            i += 1
        Next

    End Sub

    Private Sub bw_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles bw.ProgressChanged

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content
        Dim tb As TextBlock = pagina.FindName("tbProgresoWinGameStore")

        tb.Text = e.ProgressPercentage.ToString + "%"

    End Sub

    Private Async Sub bw_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bw.RunWorkerCompleted

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Await helper.SaveFileAsync(Of List(Of Juego))("listaOfertasWinGameStore", listaJuegos)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim cbOrdenar As ComboBox = pagina.FindName("cbOrdenarWinGameStore")
        Dim cbDRM As ComboBox = pagina.FindName("cbDRMWinGameStore")

        Ordenar.Ofertas("WinGameStore", cbOrdenar.SelectedIndex, Nothing, cbDRM.SelectedIndex)

    End Sub

End Module