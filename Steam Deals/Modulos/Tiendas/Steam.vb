﻿Imports System.Net
Imports Microsoft.Toolkit.Uwp.Helpers

Module Steam

    Dim WithEvents Bw As New BackgroundWorker
    Dim listaJuegos As New List(Of Juego)
    Dim listaAnalisis As New List(Of JuegoAnalisis)

    Public Async Sub GenerarOfertas()

        Dim helper As New LocalObjectStorageHelper

        If Await helper.FileExistsAsync("listaAnalisis") Then
            listaAnalisis = Await helper.ReadFileAsync(Of List(Of JuegoAnalisis))("listaAnalisis")
        End If

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

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

        Dim numPaginas As Integer = 0

        numPaginas = GenerarNumPaginas(New Uri("https://store.steampowered.com/search/?sort_by=Price_ASC&specials=1&page=1&l=english"))

        Dim i As Integer = 1
        While i < numPaginas
            Dim html_ As Task(Of String) = HttpClient(New Uri("https://store.steampowered.com/search/?cc=fr&sort_by=Price_ASC&specials=1&page=" + i.ToString + "&l=english"))
            Dim html As String = html_.Result

            If Not html = Nothing Then
                If Not html.Contains("<!-- List Items -->") Then
                    Exit While
                Else
                    Dim int0 As Integer

                    int0 = html.IndexOf("<!-- List Items -->")
                    html = html.Remove(0, int0)

                    int0 = html.IndexOf("<!-- End List Items -->")
                    html = html.Remove(int0, html.Length - int0)

                    Dim j As Integer = 0
                    While j < 50
                        If html.Contains("<a href=" + ChrW(34) + "https://store.steampowered.com/") Then
                            Dim temp, temp2 As String
                            Dim int, int2 As Integer

                            int = html.IndexOf("<a href=" + ChrW(34) + "https://store.steampowered.com/")
                            temp = html.Remove(0, int + 5)

                            html = temp

                            int2 = temp.IndexOf("</a>")
                            temp2 = temp.Remove(int2, temp.Length - int2)

                            Dim temp3, temp4 As String
                            Dim int3, int4 As Integer

                            int3 = temp2.IndexOf("<span class=" + ChrW(34) + "title" + ChrW(34) + ">")
                            temp3 = temp2.Remove(0, int3)

                            int4 = temp3.IndexOf("</span>")
                            temp4 = temp3.Remove(int4, temp3.Length - int4)

                            int4 = temp4.IndexOf(">")
                            temp4 = temp4.Remove(0, int4 + 1)

                            temp4 = temp4.Trim
                            temp4 = WebUtility.HtmlDecode(temp4)

                            Dim titulo As String = temp4

                            Dim temp5, temp6 As String
                            Dim int5, int6 As Integer

                            int5 = temp2.IndexOf("https://")
                            temp5 = temp2.Remove(0, int5)

                            int6 = temp5.IndexOf("?")
                            temp6 = temp5.Remove(int6, temp5.Length - int6)

                            Dim enlace As String = temp6.Trim

                            Dim temp7, temp8 As String
                            Dim int7, int8 As Integer

                            int7 = temp2.IndexOf("<img src=")
                            temp7 = temp2.Remove(0, int7 + 10)

                            int8 = temp7.IndexOf("?")
                            temp8 = temp7.Remove(int8, temp7.Length - int8)

                            temp8 = temp8.Trim

                            Dim imagenPequeña As String = temp8
                            imagenPequeña = imagenPequeña.Replace("capsule_sm_120", "header_292x136")

                            Dim imagenGrande As String = temp8
                            imagenGrande = imagenGrande.Replace("capsule_sm_120", "header")

                            Dim imagenes As New JuegoImagenes(imagenPequeña, imagenGrande)

                            Dim temp9, temp10 As String
                            Dim int9, int10 As Integer

                            int9 = temp2.IndexOf("col search_discount")
                            temp9 = temp2.Remove(0, int9)

                            int9 = temp9.IndexOf("<span>")
                            temp9 = temp9.Remove(0, int9 + 6)

                            int10 = temp9.IndexOf("</span>")

                            Dim descuento As String = Nothing

                            If Not int10 = -1 Then
                                temp10 = temp9.Remove(int10, temp9.Length - int10)

                                temp10 = temp10.Replace("-", Nothing)
                                temp10 = temp10.Trim

                                If temp10.Length = 2 Then
                                    temp10 = "0" + temp10
                                End If

                                descuento = temp10
                            End If

                            Dim temp11, temp12 As String
                            Dim int11, int12 As Integer

                            int11 = temp2.IndexOf("col search_price ")
                            temp11 = temp2.Remove(0, int11)

                            If Not descuento = Nothing Then
                                int11 = temp11.IndexOf("<br>")
                                temp11 = temp11.Remove(0, int11 + 4)

                                int12 = temp11.IndexOf("</div>")
                                temp12 = temp11.Remove(int12, temp11.Length - int12)
                            Else
                                int11 = temp11.IndexOf(ChrW(34) + ">")
                                temp11 = temp11.Remove(0, int11 + 2)

                                int12 = temp11.IndexOf("</div>")
                                temp12 = temp11.Remove(int12, temp11.Length - int12)
                            End If

                            Dim precio As String = temp12.Trim
                            Dim boolPrecio As Boolean = False

                            If precio.Length = 0 Then
                                boolPrecio = True
                            ElseIf precio.Contains("Free") Then
                                boolPrecio = True
                            End If

                            If boolPrecio = False Then
                                precio = precio.Replace(",", ".")

                                Dim listaEnlaces As New List(Of String) From {
                                    enlace
                                }

                                Dim listaPrecios As New List(Of String) From {
                                    precio
                                }

                                Dim enlaces As New JuegoEnlaces(Nothing, listaEnlaces, Nothing, listaPrecios)

                                Dim windows As Boolean = False

                                If temp2.Contains(ChrW(34) + "platform_img win" + ChrW(34)) Then
                                    windows = True
                                End If

                                Dim mac As Boolean = False

                                If temp2.Contains(ChrW(34) + "platform_img mac" + ChrW(34)) Then
                                    mac = True
                                End If

                                Dim linux As Boolean = False

                                If temp2.Contains(ChrW(34) + "platform_img linux" + ChrW(34)) Then
                                    linux = True
                                End If

                                Dim sistemas As New JuegoSistemas(windows, mac, linux)

                                Dim analisis As JuegoAnalisis = Nothing

                                If temp2.Contains("data-tooltip-html=") Then
                                    analisis = AñadirAnalisis(temp2, listaAnalisis)
                                End If

                                Dim temp13, temp14 As String
                                Dim int13 As Integer

                                temp13 = enlace.Replace("https://store.steampowered.com/", Nothing)
                                int13 = temp13.IndexOf("/")

                                temp14 = temp13.Remove(int13, temp13.Length - int13)

                                Dim tipo As String = temp14.Trim

                                Dim juego As New Juego(titulo, imagenes, enlaces, descuento, Nothing, "Steam", Nothing, tipo, DateTime.Today, Nothing, analisis, sistemas, Nothing)

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
                        j += 1
                    End While
                End If
            End If
            Bw.ReportProgress(CInt((100 / numPaginas) * i))
            i += 1
        End While

    End Sub

    Private Sub Bw_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles Bw.ProgressChanged

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tb As TextBlock = pagina.FindName("tbOfertasProgreso")
        tb.Text = e.ProgressPercentage.ToString + "%"

    End Sub

    Private Async Sub Bw_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles Bw.RunWorkerCompleted

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Await helper.SaveFileAsync(Of List(Of Juego))("listaOfertasSteam", listaJuegos)

        Ordenar.Ofertas("Steam", True, False)

    End Sub

    '----------------------------------------------------

    Public Function GenerarNumPaginas(url As Uri)

        Dim numPaginas As Integer = 0
        Dim htmlPaginas_ As Task(Of String) = HttpClient(url)
        Dim htmlPaginas As String = htmlPaginas_.Result

        If Not htmlPaginas = Nothing Then
            If htmlPaginas.Contains("<div class=" + ChrW(34) + "search_pagination_right" + ChrW(34) + ">") Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = htmlPaginas.IndexOf("<div class=" + ChrW(34) + "search_pagination_right" + ChrW(34) + ">")
                temp = htmlPaginas.Remove(0, int)

                int2 = temp.IndexOf("</div>")
                temp2 = temp.Remove(int2, temp.Length - int2)

                If temp2.Contains("<a href=") Then
                    Dim i As Integer = 0
                    While i < 10
                        If temp2.Contains("<a href=") Then
                            Dim temp3, temp4, temp5 As String
                            Dim int3, int4, int5 As Integer

                            int3 = temp2.IndexOf("<a href=")
                            temp3 = temp2.Remove(0, int3 + 3)

                            temp2 = temp3

                            int4 = temp3.IndexOf(">")
                            temp4 = temp3.Remove(0, int4 + 1)

                            int5 = temp4.IndexOf("</a>")
                            temp5 = temp4.Remove(int5, temp4.Length - int5)

                            If Not temp5.Contains("&gt;") Then
                                If Not temp5.Contains("&lt;") Then
                                    If Integer.Parse(temp5.Trim) > numPaginas Then
                                        numPaginas = temp5
                                    End If
                                End If
                            End If
                        End If
                        i += 1
                    End While
                Else
                    numPaginas = 5
                End If
            Else
                numPaginas = 300
            End If
        Else
            numPaginas = 300
        End If

        numPaginas = numPaginas + 1

        Return numPaginas
    End Function

    Public Async Function SteamMas(juego As Juego) As Task(Of Juego)

        Dim htmlMas As String = Await HttpClient(New Uri(juego.Enlaces.Enlaces(0)))

        If Not htmlMas = Nothing Then
            If htmlMas.Contains(ChrW(34) + "game_purchase_discount_countdown" + ChrW(34)) Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = htmlMas.IndexOf(ChrW(34) + "game_purchase_discount_countdown" + ChrW(34))
                temp = htmlMas.Remove(0, int + 10)

                int2 = temp.IndexOf("</p>")
                temp2 = temp.Remove(int2, temp.Length - int2)

                If temp2.Contains("Offer ends") Then
                    int = temp2.IndexOf("Offer ends")
                    temp2 = temp2.Remove(0, int + 10)

                    temp2 = temp2.Trim

                    Dim fecha As DateTime = Nothing

                    Try
                        fecha = DateTime.Parse(temp2)
                        fecha = fecha.AddHours(19)
                        juego.FechaTermina = fecha
                    Catch ex As Exception

                    End Try

                    Dim temp3 As String
                    Dim int3 As Integer

                    int3 = temp.IndexOf("Offer ends")
                    temp3 = temp.Remove(int3, temp.Length - int3)

                    int3 = temp3.IndexOf(">")
                    temp3 = temp3.Remove(0, int3 + 1)

                    temp3 = temp3.Replace("!", Nothing)
                    temp3 = temp3.Replace("¡", Nothing)
                    temp3 = temp3.Trim
                    temp3 = temp3.ToLower

                    juego.Promocion = temp3
                End If
            End If
        End If

        Return juego

    End Function

End Module
