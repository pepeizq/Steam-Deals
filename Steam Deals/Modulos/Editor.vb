﻿Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Syncfusion.XlsIO
Imports Windows.Storage
Imports Windows.Storage.Pickers

Module Editor

    Public Sub Generar2(listaJuegos As List(Of Juego), tienda As Tienda)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim paquete As New EditorPaquete(listaJuegos, tienda, Nothing)

        Dim imagenTienda As ImageEx = pagina.FindName("imagenEditorTienda")

        If Not paquete.Tienda Is Nothing Then
            imagenTienda.Source = paquete.Tienda.Icono

            Dim tbTienda As TextBlock = pagina.FindName("tbEditorTienda")
            tbTienda.Text = paquete.Tienda.NombreMostrar + " (" + paquete.ListaJuegos.Count.ToString + ")"
        End If

        Dim cbWebs As ComboBox = pagina.FindName("cbEditorWebs")
        Dim webSeleccionada As Integer = cbWebs.SelectedIndex

        Dim mes As String = DateTime.Now.Month.ToString

        If mes.Length = 1 Then
            mes = "0" + mes
        End If

        Dim dia As String = DateTime.Now.Day.ToString

        If dia.Length = 1 Then
            dia = "0" + dia
        End If

        Dim hora As String = DateTime.Now.Hour.ToString

        If hora.Length = 1 Then
            hora = "0" + hora
        End If

        Dim minuto As String = DateTime.Now.Minute.ToString

        If minuto.Length = 1 Then
            minuto = "0" + minuto
        End If

        Dim segundo As String = DateTime.Now.Second.ToString

        If segundo.Length = 1 Then
            segundo = "0" + segundo
        End If

        Dim nombreTablaGenerar As String = paquete.Tienda.NombreUsar.ToLower + mes + dia + hora + minuto + segundo
        paquete.NombreTabla = nombreTablaGenerar

        Dim wv As WebView = pagina.FindName("wvEditor")

        If webSeleccionada = 0 Then
            If listaJuegos.Count < 2 Then
                wv.Navigate(New Uri("https://pepeizqdeals.com/wp-admin/post-new.php?post_type=us_portfolio"))
            ElseIf listaJuegos.Count > 1 Then
                wv.Navigate(New Uri("https://pepeizqdeals.com/wp-admin/admin.php?page=wpdatatables-constructor&source"))
            End If
        End If

        imagenTienda.Tag = paquete

    End Sub

    Public Async Sub ExportarExcel()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim imagenTienda As ImageEx = pagina.FindName("imagenEditorTienda")
        Dim paquete As EditorPaquete = imagenTienda.Tag
        Dim listaJuegos As List(Of Juego) = paquete.ListaJuegos

        Dim wv As WebView = pagina.FindName("wvEditor")

        Using motor As New ExcelEngine
            motor.Excel.DefaultVersion = ExcelVersion.Excel2016

            Dim workbook As IWorkbook = motor.Excel.Workbooks.Create(1)
            Dim worksheet As IWorksheet = workbook.Worksheets(0)

            worksheet.Range("B1").Text = "Title"
            worksheet.Range("C1").Text = "Discount"

            If listaJuegos(0).Enlaces.Precios.Count = 1 Then
                worksheet.Range("D1").Text = "Price"
                worksheet.Range("E1").Text = "Reviews"
            Else
                Dim letra As Char = "D"

                Dim j As Integer = 0
                While j < listaJuegos(0).Enlaces.Paises.Count
                    worksheet.Range(letra.ToString + "1").Text = "Price (" + listaJuegos(0).Enlaces.Paises(j) + ")"

                    letra = ChrW(AscW(letra) + 1)
                    j += 1
                End While

                worksheet.Range(letra.ToString + "1").Text = "Reviews"
            End If

            Dim i As Integer = 0
            While i < listaJuegos.Count
                Dim drm As String = Nothing

                If Not listaJuegos(i).DRM = Nothing Then
                    drm = listaJuegos(i).DRM

                    If drm.ToLower.Contains("steam") Then
                        drm = "<br><span style=" + ChrW(34) + "background-color:#b9babc;color:white;padding:5px;display:inline-flex;align-items:center;" + ChrW(34) + "><img src=" + ChrW(34) + "https://pepeizqdeals.com/wp-content/uploads/2018/03/drm_steam.png" + ChrW(34) + "></span>"
                    ElseIf drm.ToLower.Contains("uplay") Then
                        drm = "<br><span style=" + ChrW(34) + "background-color:#bae5f6;color:white;padding:5px;display:inline-flex;align-items:center;" + ChrW(34) + "><img src=" + ChrW(34) + "https://pepeizqdeals.com/wp-content/uploads/2018/03/drm_uplay.png" + ChrW(34) + "></span>"
                    Else
                        drm = Nothing
                    End If
                End If

                Dim enlaceMostrar As String = Nothing

                If listaJuegos(i).Enlaces.Afiliados Is Nothing Then
                    enlaceMostrar = listaJuegos(i).Enlaces.Enlaces(0)
                Else
                    enlaceMostrar = listaJuegos(i).Enlaces.Afiliados(0)
                End If

                worksheet.Range("A" + (i + 2).ToString).Text = "<a title=" + ChrW(34) + listaJuegos(i).Titulo + ChrW(34) + " href=" + ChrW(34) + enlaceMostrar + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + " ><img src=" + ChrW(34) + listaJuegos(i).Imagenes.Pequeña + ChrW(34) + "></a>"
                worksheet.Range("B" + (i + 2).ToString).Text = "<a title=" + ChrW(34) + listaJuegos(i).Titulo + ChrW(34) + " href=" + ChrW(34) + enlaceMostrar + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + " style=" + ChrW(34) + "color:#164675;font-size:15px;" + ChrW(34) + ">" + listaJuegos(i).Titulo + drm + "</a>"
                worksheet.Range("C" + (i + 2).ToString).Text = "<a title=" + ChrW(34) + listaJuegos(i).Descuento + ChrW(34) + " href=" + ChrW(34) + enlaceMostrar + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + " ><span style=" + ChrW(34) + "background-color:green;color:white;padding:5px;font-size:15px;" + ChrW(34) + ">" + listaJuegos(i).Descuento + "</span></a>"

                Dim letra As Char = "D"

                Dim h As Integer = 0
                While h < listaJuegos(i).Enlaces.Precios.Count
                    Dim precioFinalMostrar As String = listaJuegos(i).Enlaces.Precios(h)

                    If precioFinalMostrar.Contains("£") Then
                        Dim tbLibra As TextBlock = pagina.FindName("tbDivisasLibra")
                        precioFinalMostrar = Divisas.CambioMoneda(precioFinalMostrar, tbLibra.Text)
                    End If

                    precioFinalMostrar = precioFinalMostrar.Replace("€", Nothing)
                    precioFinalMostrar = precioFinalMostrar.Replace(",", ".")
                    precioFinalMostrar = precioFinalMostrar.Trim
                    precioFinalMostrar = precioFinalMostrar + " €"

                    Dim precioFinalOrdenar As String = precioFinalMostrar
                    Dim posicionPunto As Integer = precioFinalMostrar.IndexOf(".")

                    precioFinalOrdenar = precioFinalOrdenar.Replace("€", Nothing)
                    precioFinalOrdenar = precioFinalOrdenar.Trim

                    If posicionPunto = 0 Then
                        precioFinalOrdenar = "000" + precioFinalOrdenar
                    ElseIf posicionPunto = 1 Then
                        precioFinalOrdenar = "00" + precioFinalOrdenar
                    ElseIf posicionPunto = 2 Then
                        precioFinalOrdenar = "0" + precioFinalOrdenar
                    End If

                    If listaJuegos(i).Enlaces.Afiliados Is Nothing Then
                        enlaceMostrar = listaJuegos(i).Enlaces.Enlaces(h)
                    Else
                        enlaceMostrar = listaJuegos(i).Enlaces.Afiliados(h)
                    End If

                    If listaJuegos(i).Enlaces.Precios.Count = 1 Then
                        worksheet.Range(letra.ToString + (i + 2).ToString).Text = "<a title=" + ChrW(34) + precioFinalOrdenar + ChrW(34) + " href=" + ChrW(34) + enlaceMostrar + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + "><span style=" + ChrW(34) + "background-color:black;color:white;padding:5px;font-size:15px;" + ChrW(34) + ">" + precioFinalMostrar + "</span></a>"
                    Else
                        Dim imagenMostrar As String = Nothing

                        If listaJuegos(i).Enlaces.Paises(h).Contains("EU") Then
                            imagenMostrar = "<img style=" + ChrW(34) + "height:16px;width:22px;margin-right:5px;" + ChrW(34) + " src=" + ChrW(34) + "https://pepeizqdeals.com/wp-content/uploads/2018/03/pais_ue2.png" + ChrW(34) + ">"
                        ElseIf listaJuegos(i).Enlaces.Paises(h).Contains("UK") Then
                            imagenMostrar = "<img style=" + ChrW(34) + "height:16px;width:22px;margin-right:5px;" + ChrW(34) + " src=" + ChrW(34) + "https://pepeizqdeals.com/wp-content/uploads/2018/03/pais_uk2.png" + ChrW(34) + ">"
                        ElseIf listaJuegos(i).Enlaces.Paises(h).Contains("FR") Then
                            imagenMostrar = "<img style=" + ChrW(34) + "height:16px;width:22px;margin-right:5px;" + ChrW(34) + " src=" + ChrW(34) + "https://pepeizqdeals.com/wp-content/uploads/2018/03/pais_fr2.png" + ChrW(34) + ">"
                        ElseIf listaJuegos(i).Enlaces.Paises(h).Contains("DE") Then
                            imagenMostrar = "<img style=" + ChrW(34) + "height:16px;width:22px;margin-right:5px;" + ChrW(34) + " src=" + ChrW(34) + "https://pepeizqdeals.com/wp-content/uploads/2018/03/pais_de2.png" + ChrW(34) + ">"
                        End If

                        worksheet.Range(letra.ToString + (i + 2).ToString).Text = "<a title=" + ChrW(34) + precioFinalOrdenar + ChrW(34) + " href=" + ChrW(34) + enlaceMostrar + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + "><span style=" + ChrW(34) + "background-color:black;color:white;padding-left:5px;padding-right:5px;padding-top:2px;padding-bottom:2px;font-size:15px;display:inline-flex;align-items:center;" + ChrW(34) + ">" + imagenMostrar + precioFinalMostrar + "</span></a>"
                    End If

                    letra = ChrW(AscW(letra) + 1)
                    h += 1
                End While

                If Not listaJuegos(i).Analisis Is Nothing Then
                    Dim imagenUrl As String = Nothing
                    Dim colorFondo As String = Nothing
                    Dim colorLetra As String = Nothing

                    If listaJuegos(i).Analisis.Porcentaje > 74 Then
                        imagenUrl = "https://pepeizqdeals.com/wp-content/uploads/2018/03/positive.png"
                        colorFondo = "#ABCADB"
                        colorLetra = "#294B5F"
                    ElseIf listaJuegos(i).Analisis.Porcentaje > 49 And listaJuegos(i).Analisis.Porcentaje < 75 Then
                        imagenUrl = "https://pepeizqdeals.com/wp-content/uploads/2018/03/mixed.png"
                        colorFondo = "#d5cbbc"
                        colorLetra = "#544834"
                    ElseIf listaJuegos(i).Analisis.Porcentaje < 50 Then
                        imagenUrl = "https://pepeizqdeals.com/wp-content/uploads/2018/03/negative.png"
                        colorFondo = "#ceb9b4"
                        colorLetra = "#631502"
                    End If

                    Dim cantidadAnalisisOrdenar As String = listaJuegos(i).Analisis.Cantidad
                    cantidadAnalisisOrdenar = cantidadAnalisisOrdenar.Replace(",", Nothing)
                    cantidadAnalisisOrdenar = cantidadAnalisisOrdenar.Replace(".", Nothing)

                    If cantidadAnalisisOrdenar.Length = 3 Then
                        cantidadAnalisisOrdenar = "0000000" + cantidadAnalisisOrdenar
                    ElseIf cantidadAnalisisOrdenar.Length = 4 Then
                        cantidadAnalisisOrdenar = "000000" + cantidadAnalisisOrdenar
                    ElseIf cantidadAnalisisOrdenar.Length = 5 Then
                        cantidadAnalisisOrdenar = "00000" + cantidadAnalisisOrdenar
                    ElseIf cantidadAnalisisOrdenar.Length = 6 Then
                        cantidadAnalisisOrdenar = "0000" + cantidadAnalisisOrdenar
                    ElseIf cantidadAnalisisOrdenar.Length = 7 Then
                        cantidadAnalisisOrdenar = "000" + cantidadAnalisisOrdenar
                    ElseIf cantidadAnalisisOrdenar.Length = 8 Then
                        cantidadAnalisisOrdenar = "00" + cantidadAnalisisOrdenar
                    ElseIf cantidadAnalisisOrdenar.Length = 9 Then
                        cantidadAnalisisOrdenar = "0" + cantidadAnalisisOrdenar
                    End If

                    Dim enlaceAnalisis As String = listaJuegos(i).Analisis.Enlace

                    If Not listaJuegos(i).Enlaces.Afiliados Is Nothing Then
                        enlaceAnalisis = listaJuegos(i).Enlaces.Afiliados(0)
                    End If

                    If enlaceAnalisis = Nothing Then
                        enlaceAnalisis = listaJuegos(i).Enlaces.Enlaces(0)
                    End If

                    worksheet.Range(letra.ToString + (i + 2).ToString).Text = "<a title=" + ChrW(34) + listaJuegos(i).Analisis.Porcentaje + " " + cantidadAnalisisOrdenar + ChrW(34) + " href=" + ChrW(34) + enlaceAnalisis + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) + " style=" + ChrW(34) + "font-size:15px;color:" + colorLetra + ";" + ChrW(34) + "><span style=" + ChrW(34) + "background-color:" + colorFondo + ";display:inline-flex;align-items:center;padding-left:5px;padding-right:5px;padding-top:2px;padding-bottom:2px;" + ChrW(34) + "><img src=" + ChrW(34) + imagenUrl + ChrW(34) + " style=" + ChrW(34) + "margin-right:5px;" + ChrW(34) + ">" + listaJuegos(i).Analisis.Porcentaje + "%</span></a>"
                End If

                i += 1
            End While

            Dim ficherosExcel As New List(Of String) From {
                ".xlsx"
            }

            Dim fichero As StorageFile = Nothing

            Dim guardarPicker As New FileSavePicker With {
                .SuggestedStartLocation = PickerLocationId.Desktop,
                .SuggestedFileName = paquete.NombreTabla
            }

            guardarPicker.FileTypeChoices.Add("Excel Files", ficherosExcel)
            fichero = Await guardarPicker.PickSaveFileAsync

            If Not fichero Is Nothing Then
                Dim stream As Stream = Await fichero.OpenStreamForWriteAsync
                workbook.SaveAs(stream)
                workbook.Close()
            End If
        End Using

    End Sub

    Public Async Sub CargaWeb(wv As WebView)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim imagenTienda As ImageEx = pagina.FindName("imagenEditorTienda")
        Dim paquete As EditorPaquete = imagenTienda.Tag

        Dim nombreTablaGenerar As String = paquete.NombreTabla

        If wv.Source = New Uri("https://pepeizqdeals.com/wp-admin/post-new.php?post_type=us_portfolio") Then

        ElseIf wv.Source = New Uri("https://pepeizqdeals.com/wp-admin/admin.php?page=wpdatatables-constructor&source") Then
            Dim lista As New List(Of String) From {
                "document.getElementById('wdt-table-title-edit').value = '" + nombreTablaGenerar + "';"
            }

            Dim argumentos As IEnumerable(Of String) = lista

            Try
                Await wv.InvokeScriptAsync("eval", argumentos)
            Catch ex As Exception

            End Try

            Dim lista2 As New List(Of String) From {
                "document.getElementsByClassName('btn dropdown-toggle bs-placeholder btn-default')[0].click();"
            }

            Dim argumentos2 As IEnumerable(Of String) = lista2

            Try
                Await wv.InvokeScriptAsync("eval", argumentos2)
            Catch ex As Exception

            End Try

        End If

    End Sub

    Public Async Sub InsertarHtml()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim imagenTienda As ImageEx = pagina.FindName("imagenEditorTienda")
        Dim paquete As EditorPaquete = imagenTienda.Tag

        Dim wv As WebView = pagina.FindName("wvEditor")

        If wv.Source = New Uri("https://pepeizqdeals.com/wp-admin/post-new.php?post_type=us_portfolio") Then
            Dim html As String = "[vc_row][vc_column][wpdatatable id=" + ChrW(34) + "28" + ChrW(34) + "][/vc_column][/vc_row]"

            Dim lista As New List(Of String) From {
                "document.getElementById('content-html').click();"
            }

            Dim argumentos As IEnumerable(Of String) = lista

            Try
                Await wv.InvokeScriptAsync("eval", argumentos)
            Catch ex As Exception

            End Try

            Dim lista2 As New List(Of String) From {
                "document.getElementById('content').value = '" + html + "';"
            }

            Dim argumentos2 As IEnumerable(Of String) = lista2

            Try
                Await wv.InvokeScriptAsync("eval", argumentos2)
            Catch ex As Exception

            End Try

            Dim listaJuegos As List(Of Juego) = paquete.ListaJuegos

            listaJuegos.Sort(Function(x As Juego, y As Juego)
                                 Dim resultado As Integer = y.Descuento.CompareTo(x.Descuento)
                                 If resultado = 0 Then
                                     resultado = x.Titulo.CompareTo(y.Titulo)
                                 End If
                                 Return resultado
                             End Function)

            Dim titulo As String = " Sale (" + listaJuegos(listaJuegos.Count - 1).Descuento + "-" + listaJuegos(0).Descuento + ") in " + paquete.Tienda.NombreMostrar + " (" + listaJuegos.Count.ToString + " Deals)"

            Dim lista3 As New List(Of String) From {
                "document.getElementById('title').value = '" + titulo + "';"
            }

            Dim argumentos3 As IEnumerable(Of String) = lista3

            Try
                Await wv.InvokeScriptAsync("eval", argumentos3)
            Catch ex As Exception

            End Try

            Dim lista4 As New List(Of String) From {
                "document.getElementById('in-us_portfolio_category-6').click();"
            }

            Dim argumentos4 As IEnumerable(Of String) = lista4

            Try
                Await wv.InvokeScriptAsync("eval", argumentos4)
            Catch ex As Exception

            End Try

            Dim lista5 As New List(Of String) From {
                "document.getElementById('enable-expirationdate').click();"
            }

            Dim argumentos5 As IEnumerable(Of String) = lista5

            Try
                Await wv.InvokeScriptAsync("eval", argumentos5)
            Catch ex As Exception

            End Try

        End If

    End Sub







    Public Async Sub Borrar()

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaFinal As New List(Of Juego)

        Await helper.SaveFileAsync(Of List(Of Juego))("listaEditorFinal", listaFinal)

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tbNumEnlaces As TextBlock = pagina.FindName("tbEditorEnlacesNum")
        tbNumEnlaces.Text = listaFinal.Count.ToString + " " + recursos.GetString("Ofertas")

        Dim tbTienda As TextBlock = pagina.FindName("tbEditorEnlacesTienda")
        tbTienda.Text = String.Empty

        Dim tbTitulo As TextBox = pagina.FindName("tbEditorTitulo")
        tbTitulo.Text = String.Empty

        Dim tbEnlaces As TextBox = pagina.FindName("tbEditorEnlaces")
        tbEnlaces.Text = String.Empty
        tbEnlaces.Visibility = Visibility.Visible

        Dim tbLimite As TextBlock = pagina.FindName("tbEditorEnlacesLimite")
        tbLimite.Visibility = Visibility.Collapsed

        Dim tbEtiquetas As TextBox = pagina.FindName("tbEditorEtiquetas")
        tbEtiquetas.Text = String.Empty

        Dim tbNumCaracteres As TextBlock = pagina.FindName("tbEditorNumCaracteres")
        tbNumCaracteres.Text = 0

    End Sub

    Public Sub Generar(lv As ListView)

        Dim recursos As New Resources.ResourceLoader()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim cbWebs As ComboBox = pagina.FindName("cbEditorWebs")
        ApplicationData.Current.LocalSettings.Values("editorWeb") = cbWebs.SelectedIndex

        If Not lv Is Nothing Then
            If lv.Items.Count > 0 Then
                Dim listaFinal As New List(Of Juego)

                For Each item In lv.Items
                    Dim itemGrid As Grid = item
                    Dim sp As StackPanel = itemGrid.Children(0)
                    Dim cb As CheckBox = sp.Children(0)

                    If cb.IsChecked = True Then
                        listaFinal.Add(itemGrid.Tag)
                    End If
                Next

                If listaFinal.Count > 0 Then
                    Dim tbLibra As MenuFlyoutItem = pagina.FindName("itemDivisasLibra")
                    Dim tbDolar As MenuFlyoutItem = pagina.FindName("itemDivisasDolar")

                    Dim contenidoEnlaces As String = Nothing

                    Dim cantidadJuegos As String = Nothing

                    If listaFinal.Count > 99 And listaFinal.Count < 200 Then
                        cantidadJuegos = "+100"
                    ElseIf listaFinal.Count > 199 And listaFinal.Count < 300 Then
                        cantidadJuegos = "+200"
                    ElseIf listaFinal.Count > 299 And listaFinal.Count < 400 Then
                        cantidadJuegos = "+300"
                    ElseIf listaFinal.Count > 399 And listaFinal.Count < 500 Then
                        cantidadJuegos = "+400"
                    ElseIf listaFinal.Count > 499 And listaFinal.Count < 600 Then
                        cantidadJuegos = "+500"
                    ElseIf listaFinal.Count > 599 And listaFinal.Count < 700 Then
                        cantidadJuegos = "+600"
                    ElseIf listaFinal.Count > 699 And listaFinal.Count < 800 Then
                        cantidadJuegos = "+700"
                    ElseIf listaFinal.Count > 799 And listaFinal.Count < 900 Then
                        cantidadJuegos = "+800"
                    ElseIf listaFinal.Count > 899 And listaFinal.Count < 1000 Then
                        cantidadJuegos = "+900"
                    ElseIf listaFinal.Count > 999 Then
                        cantidadJuegos = "+1000"
                    Else
                        cantidadJuegos = listaFinal.Count.ToString
                    End If

                    Dim gridReddit As Grid = pagina.FindName("gridEditorReddit")
                    Dim gridVayaAnsias As Grid = pagina.FindName("gridEditorVayaAnsias")

                    If cbWebs.SelectedIndex = 0 Then
                        gridReddit.Visibility = Visibility.Visible
                        gridVayaAnsias.Visibility = Visibility.Collapsed

                        Dim tbTitulo As TextBox = pagina.FindName("tbEditorTituloReddit")

                        If listaFinal.Count = 0 Then
                            tbTitulo.Text = String.Empty
                        ElseIf listaFinal.Count = 1 Then
                            tbTitulo.Text = "[" + listaFinal(0).Tienda + "] " + listaFinal(0).Titulo + " (" + listaFinal(0).Enlaces.Precios(0) + "/" + listaFinal(0).Descuento + " off)"
                        Else
                            tbTitulo.Text = "[" + listaFinal(0).Tienda + "] Sale | Up to " + listaFinal(0).Descuento + " off (" + cantidadJuegos + " deals)"
                        End If

                        If listaFinal(0).Tienda = "Steam" Then
                            contenidoEnlaces = contenidoEnlaces + "**Title** | **Discount** | **Price** | **Rating**" + Environment.NewLine
                            contenidoEnlaces = contenidoEnlaces + ":--------|:---------:|:---------:|:---------:" + Environment.NewLine
                        ElseIf listaFinal(0).Tienda = "GOG" Then
                            contenidoEnlaces = contenidoEnlaces + "**Title** | **Discount** | **Price** | **Rating**" + Environment.NewLine
                            contenidoEnlaces = contenidoEnlaces + ":--------|:---------:|:---------:|:---------:" + Environment.NewLine
                        ElseIf listaFinal(0).Tienda = "Microsoft Store" Then
                            contenidoEnlaces = contenidoEnlaces + "**Title** | **Discount** | **Price** | **Rating**" + Environment.NewLine
                            contenidoEnlaces = contenidoEnlaces + ":--------|:---------:|:---------:|:---------:" + Environment.NewLine
                        ElseIf listaFinal(0).Tienda = "GamersGate" Then
                            contenidoEnlaces = contenidoEnlaces + "**Title** | **DRM** | **Discount** | **Price EU** | **Price UK** | **Rating**" + Environment.NewLine
                            contenidoEnlaces = contenidoEnlaces + ":--------|:--------:|:---------:|:---------:|:---------:|:---------:" + Environment.NewLine
                        ElseIf listaFinal(0).Tienda = "GamesPlanet" Then
                            contenidoEnlaces = contenidoEnlaces + "**Title** | **DRM** | **Discount** | **Price UK** | **Price FR** | **Price DE** | **Rating**" + Environment.NewLine
                            contenidoEnlaces = contenidoEnlaces + ":--------|:--------:|:---------:|:---------:|:---------:|:---------:|:---------:" + Environment.NewLine
                        ElseIf listaFinal(0).Tienda = "Fanatical" Then
                            contenidoEnlaces = contenidoEnlaces + "**Title** | **DRM** | **Discount** | **Price EU** | **Price US** | **Price UK** | **Rating**" + Environment.NewLine
                            contenidoEnlaces = contenidoEnlaces + ":--------|:--------:|:---------:|:---------:|:---------:|:---------:|:---------:" + Environment.NewLine
                        Else
                            contenidoEnlaces = contenidoEnlaces + "**Title** | **DRM** | **Discount** | **Price** | **Rating**" + Environment.NewLine
                            contenidoEnlaces = contenidoEnlaces + ":--------|:--------:|:---------:|:---------:|:---------:" + Environment.NewLine
                        End If

                        For Each juego In listaFinal
                            Dim drm As String = Nothing
                            If Not juego.DRM = Nothing Then
                                If juego.DRM.ToLower.Contains("steam") Then
                                    drm = "Steam"
                                ElseIf juego.DRM.ToLower.Contains("uplay") Then
                                    drm = "Uplay"
                                ElseIf juego.DRM.ToLower.Contains("origin") Then
                                    drm = "Origin"
                                ElseIf juego.DRM.ToLower.Contains("gog") Then
                                    drm = "GOG"
                                End If
                            End If

                            Dim analisis As String = Nothing

                            If Not juego.Analisis Is Nothing Then
                                If Not juego.Analisis.Enlace = Nothing Then
                                    analisis = "[" + juego.Analisis.Porcentaje + "](" + juego.Analisis.Enlace + ")"
                                Else
                                    analisis = juego.Analisis.Porcentaje
                                End If
                            Else
                                analisis = "--"
                            End If

                            Dim linea As String = Nothing

                            If listaFinal(0).Tienda = "Steam" Then
                                linea = linea + "[" + juego.Titulo + "](" + juego.Enlaces.Enlaces(0) + ") | " + juego.Descuento + " | " + juego.Enlaces.Precios(0) + " | " + analisis
                            ElseIf listaFinal(0).Tienda = "GOG" Then
                                linea = linea + "[" + juego.Titulo + "](" + juego.Enlaces.Enlaces(0) + ") | " + juego.Descuento + " | " + juego.Enlaces.Precios(0) + " | " + analisis
                            ElseIf listaFinal(0).Tienda = "Microsoft Store" Then
                                linea = linea + "[" + juego.Titulo + "](" + juego.Enlaces.Enlaces(0) + ") | " + juego.Descuento + " | " + juego.Enlaces.Precios(0) + " | " + analisis
                            ElseIf listaFinal(0).Tienda = "GamersGate" Then
                                linea = linea + juego.Titulo + " | " + drm + " | " + juego.Descuento + " | [" + juego.Enlaces.Precios(0) + "](" + juego.Enlaces.Enlaces(0) + ") | [" + juego.Enlaces.Precios(1) + "](" + juego.Enlaces.Enlaces(1) + ") | " + analisis
                            ElseIf listaFinal(0).Tienda = "GamesPlanet" Then
                                linea = linea + juego.Titulo + " | " + drm + " | " + juego.Descuento + " | [" + juego.Enlaces.Precios(0) + " (" + Divisas.CambioMoneda(juego.Enlaces.Precios(0), tbLibra.Text) + ")](" + juego.Enlaces.Enlaces(0) + ") | [" + juego.Enlaces.Precios(1) + "](" + juego.Enlaces.Enlaces(1) + ") | [" + juego.Enlaces.Precios(2) + "](" + juego.Enlaces.Enlaces(2) + ")" + " | " + analisis
                            ElseIf listaFinal(0).Tienda = "Fanatical" Then
                                linea = linea + "[" + juego.Titulo + "](" + juego.Enlaces.Enlaces(0) + ") | " + drm + " | " + juego.Descuento + " | " + juego.Enlaces.Precios(1) + " | " + juego.Enlaces.Precios(0) + " | " + juego.Enlaces.Precios(2) + " | " + analisis
                            Else
                                linea = linea + "[" + juego.Titulo + "](" + juego.Enlaces.Enlaces(0) + ") | " + drm + " | " + juego.Descuento + " | " + juego.Enlaces.Precios(0) + " | " + analisis
                            End If

                            If Not linea = Nothing Then
                                contenidoEnlaces = contenidoEnlaces + linea + Environment.NewLine
                            End If
                        Next

                        Dim tbEnlaces As TextBox = pagina.FindName("tbEditorEnlacesReddit")
                        tbEnlaces.Tag = contenidoEnlaces

                        If contenidoEnlaces.Length < 40000 Then
                            tbEnlaces.Text = contenidoEnlaces
                        Else
                            tbEnlaces.Text = recursos.GetString("EditorLimit")
                        End If

                    ElseIf cbWebs.SelectedIndex = 1 Then
                        gridReddit.Visibility = Visibility.Collapsed
                        gridVayaAnsias.Visibility = Visibility.Visible

                        Dim tbTitulo As TextBox = pagina.FindName("tbEditorTituloVayaAnsias")

                        If listaFinal.Count = 0 Then
                            tbTitulo.Text = String.Empty
                        ElseIf listaFinal.Count = 1 Then
                            If listaFinal(0).Tienda = "Amazon.es" Then
                                tbTitulo.Text = listaFinal(0).Titulo + " a " + listaFinal(0).Enlaces.Precios(0).Replace(" ", Nothing) + " en " + Twitter(listaFinal(0).Tienda) + " (para #Steam) - Formato Físico"
                            Else
                                Dim drm As String = Nothing

                                If Not listaFinal(0).DRM = Nothing Then
                                    If listaFinal(0).DRM.ToLower.Contains("steam") Then
                                        drm = " (para #Steam)"
                                    ElseIf listaFinal(0).DRM.ToLower.Contains("uplay") Then
                                        drm = " (para #Uplay)"
                                    ElseIf listaFinal(0).DRM.ToLower.Contains("origin") Then
                                        drm = " (para #Origin)"
                                    ElseIf listaFinal(0).DRM.ToLower.Contains("gog") Then
                                        drm = " (para #GOGcom)"
                                    End If
                                End If

                                tbTitulo.Text = listaFinal(0).Titulo + " al " + listaFinal(0).Descuento + " en " + Twitter(listaFinal(0).Tienda) + drm
                            End If
                        Else
                            Dim descuentoBajo As String = listaFinal(listaFinal.Count - 1).Descuento.Replace("%", Nothing)
                            Dim descuentoTop As String = listaFinal(0).Descuento

                            tbTitulo.Text = listaFinal.Count.ToString + " juegos para #Steam en " + Twitter(listaFinal(0).Tienda) + " (" + descuentoBajo + "-" + descuentoTop + ")"
                        End If

                        contenidoEnlaces = contenidoEnlaces + "<br/><div style=" + ChrW(34) + "text-align:center;" + ChrW(34) + ">" + Environment.NewLine

                        Dim enlaceImagen As String = Nothing

                        If Not listaFinal(0).Enlaces.Afiliados Is Nothing Then
                            enlaceImagen = listaFinal(0).Enlaces.Afiliados(0)
                        Else
                            enlaceImagen = listaFinal(0).Enlaces.Enlaces(0)
                        End If

                        Dim imagen As String = Nothing

                        If listaFinal(0).Tienda = "Amazon.es" Then
                            imagen = listaFinal(0).Imagenes.Grande

                            imagen = imagen + ChrW(34) + " Width=" + ChrW(34) + "20%"
                        Else
                            If Not listaFinal(0).Imagenes.Grande = Nothing Then
                                imagen = listaFinal(0).Imagenes.Grande
                            Else
                                imagen = listaFinal(0).Imagenes.Pequeña
                            End If
                        End If

                        contenidoEnlaces = contenidoEnlaces + "<a href=" + ChrW(34) + enlaceImagen + ChrW(34) + " target=" + ChrW(34) + "_blank" + ChrW(34) +
                            "><img src=" + ChrW(34) + imagen + ChrW(34) + "/></a></div>"

                        contenidoEnlaces = contenidoEnlaces + "<br/><ul>" + Environment.NewLine

                        Dim i As Integer = 0

                        For Each juego In listaFinal
                            i += 1

                            If i = 21 Then
                                contenidoEnlaces = contenidoEnlaces + "<!--more-->" + Environment.NewLine
                            End If

                            Dim descuento As String = Nothing

                            If Not juego.Descuento = Nothing Then
                                descuento = juego.Descuento + " - "
                            End If

                            Dim drm As String = Nothing

                            If Not juego.DRM = Nothing Then
                                If juego.DRM.ToLower.Contains("steam") Then
                                    drm = " (<font color=" + ChrW(34) + "#E56717" + ChrW(34) + ">Steam</font>)"
                                ElseIf juego.DRM.ToLower.Contains("uplay") Then
                                    drm = " (<font color=" + ChrW(34) + "#e11d9a" + ChrW(34) + ">Uplay</font>)"
                                ElseIf juego.DRM.ToLower.Contains("origin") Then
                                    drm = " (<font color=" + ChrW(34) + "#FF0000" + ChrW(34) + ">Origin</font>)"
                                ElseIf juego.DRM.ToLower.Contains("gog") Then
                                    drm = " (<font color=" + ChrW(34) + "#2EFEC8" + ChrW(34) + ">GOG</font>)"
                                End If
                            End If

                            If juego.Tienda = "Amazon.es" Then
                                drm = " (<font color=" + ChrW(34) + "#E56717" + ChrW(34) + ">Steam</font>)"
                            End If

                            If juego.Tienda = "GamersGate" Then
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(1) + ChrW(34) + ">" +
                                   descuento + juego.Titulo + " {UK}</a> - " + juego.Enlaces.Precios(1) + " (o " + Divisas.CambioMoneda(juego.Enlaces.Precios(1), tbLibra.Text) + ")" + drm +
                                   "</li>" + Environment.NewLine
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(0) + ChrW(34) + ">" +
                                   descuento + juego.Titulo + "</a> - " + juego.Enlaces.Precios(0) + drm +
                                   "</li>" + Environment.NewLine
                            ElseIf juego.Tienda = "GamesPlanet" Then
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(0) + ChrW(34) + ">" +
                                   descuento + juego.Titulo + " {UK}</a> - " + juego.Enlaces.Precios(0) + " (o " + Divisas.CambioMoneda(juego.Enlaces.Precios(0), tbLibra.Text) + ")" + drm +
                                   "</li>" + Environment.NewLine
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(1) + ChrW(34) + ">" +
                                   descuento + juego.Titulo + " {FR}</a> - " + juego.Enlaces.Precios(1) + drm +
                                   "</li>" + Environment.NewLine
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(2) + ChrW(34) + ">" +
                                   descuento + juego.Titulo + " {DE}</a> - " + juego.Enlaces.Precios(2) + drm +
                                   "</li>" + Environment.NewLine
                            ElseIf juego.Tienda = "WinGameStore" Then
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(0) + ChrW(34) + ">" +
                                   descuento + juego.Titulo + "</a> - " + juego.Enlaces.Precios(0) + " (o " + Divisas.CambioMoneda(juego.Enlaces.Precios(0), tbDolar.Text) + ")" + drm +
                                   "</li>" + Environment.NewLine
                            ElseIf juego.Tienda = "Fanatical" Then
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(0) + ChrW(34) + ">" +
                                   descuento + juego.Titulo + "</a> - " + juego.Enlaces.Precios(1) + drm +
                                   "</li>" + Environment.NewLine
                            ElseIf juego.Tienda = "Amazon.es" Then
                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + juego.Enlaces.Afiliados(0) + ChrW(34) + ">" +
                                   juego.Titulo + "</a> - " + juego.Enlaces.Precios(0) + drm + "</li>" + Environment.NewLine
                            Else
                                Dim enlace As String = Nothing
                                If Not juego.Enlaces.Afiliados Is Nothing Then
                                    enlace = juego.Enlaces.Afiliados(0)
                                Else
                                    enlace = juego.Enlaces.Enlaces(0)
                                End If

                                contenidoEnlaces = contenidoEnlaces + "<li><a href=" + ChrW(34) + enlace + ChrW(34) + ">" +
                                   descuento + juego.Titulo + "</a> - " + juego.Enlaces.Precios(0) + drm +
                                   "</li>" + Environment.NewLine
                            End If
                        Next

                        contenidoEnlaces = contenidoEnlaces + "</ul><br/>"

                        Dim tbEnlaces As TextBox = pagina.FindName("tbEditorEnlacesVayaAnsias")
                        tbEnlaces.Tag = contenidoEnlaces

                        If contenidoEnlaces.Length < 40000 Then
                            tbEnlaces.Text = contenidoEnlaces
                        Else
                            tbEnlaces.Text = recursos.GetString("EditorLimit")
                        End If

                        Dim tbEtiquetas As TextBox = pagina.FindName("tbEditorEtiquetasVayaAnsias")

                        If listaFinal(0).Tienda = "Amazon.es" Then
                            tbEtiquetas.Text = "Amazon, oferta, Formato Físico,"
                        ElseIf listaFinal(0).Tienda = "GOG" Then
                            tbEtiquetas.Text = "GOG, oferta, DRM-Free, "
                        ElseIf listaFinal(0).Tienda = "Green Man Gaming" Then
                            tbEtiquetas.Text = "GMG, GreenManGaming, oferta,"
                        Else
                            tbEtiquetas.Text = listaFinal(0).Tienda + ", oferta,"
                        End If

                        Dim notas As String = Nothing

                        notas = notas + " (<font color=" + ChrW(34) + "#E56717" + ChrW(34) + ">Steam</font>)" + Environment.NewLine
                        notas = notas + " (<font color=" + ChrW(34) + "#e11d9a" + ChrW(34) + ">Uplay</font>)" + Environment.NewLine
                        notas = notas + " (<font color=" + ChrW(34) + "#FF0000" + ChrW(34) + ">Origin</font>)" + Environment.NewLine
                        notas = notas + " (<font color=" + ChrW(34) + "#2EFEC8" + ChrW(34) + ">GOG</font>)" + Environment.NewLine
                        notas = notas + " (<font color=" + ChrW(34) + "#B298FF" + ChrW(34) + ">Battle.net</font>)"

                        Dim tbNotas As TextBox = pagina.FindName("tbEditorNotasVayaAnsias")
                        tbNotas.Text = notas

                    End If
                End If
            End If
        End If

    End Sub

    Private Function Twitter(tienda As String)

        If tienda = "Amazon.es" Then
            tienda = "@AmazonESP"
        ElseIf tienda = "Bundle Stars" Then
            tienda = "@BundleStars"
        ElseIf tienda = "GamersGate" Then
            tienda = "@GamersGate"
        ElseIf tienda = "GamesPlanet" Then
            tienda = "@GamesPlanetUK"
        ElseIf tienda = "GOG" Then
            tienda = "@GOGcom"
        ElseIf tienda = "Green Man Gaming" Then
            tienda = "@GreenManGaming"
        ElseIf tienda = "Humble Store" Then
            tienda = "@humblestore"
        ElseIf tienda = "Microsoft Store" Then
            tienda = "@MicrosoftStore"
        ElseIf tienda = "Steam" Then
            tienda = "@steam_games"
        ElseIf tienda = "WinGameStore" Then
            tienda = "@wingamestore"
        End If

        Return tienda
    End Function

End Module
