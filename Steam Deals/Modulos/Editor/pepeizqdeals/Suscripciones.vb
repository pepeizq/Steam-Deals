﻿Imports System.Globalization
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Steam_Deals.pepeizq.Suscripciones
Imports Windows.ApplicationModel.DataTransfer

Namespace pepeizq.Editor.pepeizqdeals
    Module Suscripciones

        Public Sub Cargar()

            BloquearControles(False)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim cbTiendas As ComboBox = pagina.FindName("cbEditorpepeizqdealsSubscriptionsTiendas")
            cbTiendas.Items.Clear()

            cbTiendas.Items.Add("--")
            cbTiendas.Items.Add("Humble Choice")
            cbTiendas.Items.Add("Prime Gaming")
            cbTiendas.Items.Add("Xbox Game Pass")
            cbTiendas.Items.Add("EA Play")
            cbTiendas.Items.Add("EA Play Pro")
            cbTiendas.Items.Add("Humble Trove")
            cbTiendas.Items.Add("Geforce Now")

            cbTiendas.SelectedIndex = 0

            RemoveHandler cbTiendas.SelectionChanged, AddressOf GenerarDatos
            AddHandler cbTiendas.SelectionChanged, AddressOf GenerarDatos

            Dim tbIDs As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsIDs")
            tbIDs.Text = String.Empty
            tbIDs.Visibility = Visibility.Collapsed

            RemoveHandler tbIDs.TextChanged, AddressOf LimpiarTexto
            AddHandler tbIDs.TextChanged, AddressOf LimpiarTexto

            Dim tbTitulo As TextBox = pagina.FindName("tbEditorTitulopepeizqdealsSubscriptions")
            tbTitulo.Text = String.Empty

            Dim tbJuegos As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsJuegos")
            tbJuegos.Text = String.Empty

            Dim tbImagenesGrid As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsEnlacesImagenGrid")
            tbImagenesGrid.Text = String.Empty

            RemoveHandler tbImagenesGrid.TextChanged, AddressOf GenerarImagenesJuegos
            AddHandler tbImagenesGrid.TextChanged, AddressOf GenerarImagenesJuegos

            Dim fechaDefecto As DateTime = DateTime.Now
            fechaDefecto = fechaDefecto.AddMonths(1)

            Dim fechaPicker As DatePicker = pagina.FindName("fechaEditorpepeizqdealsSubscriptions")
            fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, 1)

            RemoveHandler fechaPicker.SelectedDateChanged, AddressOf CambioFechaAviso
            AddHandler fechaPicker.SelectedDateChanged, AddressOf CambioFechaAviso

            Dim horaPicker As TimePicker = pagina.FindName("horaEditorpepeizqdealsSubscriptions")
            horaPicker.SelectedTime = New TimeSpan(fechaDefecto.Hour, 0, 0)

            Dim botonCopiarHtml As Button = pagina.FindName("botonEditorCopiarHtmlpepeizqdealsSubscriptions")

            RemoveHandler botonCopiarHtml.Click, AddressOf CopiarHtml
            AddHandler botonCopiarHtml.Click, AddressOf CopiarHtml

            Dim botonSubir As Button = pagina.FindName("botonEditorSubirpepeizqdealsSubscriptions")

            RemoveHandler botonSubir.Click, AddressOf GenerarDatos2
            AddHandler botonSubir.Click, AddressOf GenerarDatos2

            BloquearControles(True)

        End Sub

        Private Sub GenerarDatos(sender As Object, e As SelectionChangedEventArgs)

            BloquearControles(False)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim cbTiendas As ComboBox = sender
            cbTiendas.IsEnabled = False

            Dim fechaDefecto As DateTime = DateTime.Now
            Dim fechaPicker As DatePicker = pagina.FindName("fechaEditorpepeizqdealsSubscriptions")

            Dim tbTitulo As TextBox = pagina.FindName("tbEditorTitulopepeizqdealsSubscriptions")

            Dim botonBuscar As Button = pagina.FindName("botonEditorpepeizqdealsSubscriptionsBuscar")
            Dim tbIDs As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsIDs")

            Dim imagenTienda1 As ImageEx = pagina.FindName("imagenTiendaEditorpepeizqdealsGenerarImagenUnaSuscripcion")
            Dim imagenTienda2 As ImageEx = pagina.FindName("imagenTiendaEditorpepeizqdealsGenerarImagenSuscripcionesv3")

            Dim cosas As New Clases.Suscripciones(Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)

            If cbTiendas.SelectedIndex = 0 Then
                botonBuscar.Visibility = Visibility.Collapsed
                tbIDs.Visibility = Visibility.Collapsed
            ElseIf cbTiendas.SelectedIndex = 1 Then
                botonBuscar.Visibility = Visibility.Visible
                tbIDs.Visibility = Visibility.Visible
                tbIDs.Text = String.Empty

                imagenTienda1.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/humblechoice.png"
                imagenTienda2.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/humblechoice.png"

                cosas.Tienda = Tiendas.humbleT
                cosas.Tienda.NombreMostrar = "Humble Bundle"
                cosas.Enlace = "https://www.humblebundle.com/subscription"

                Dim ci As CultureInfo = New CultureInfo("en-US")
                Dim mes As String = DateTime.Now.ToString("MMMM", ci)
                cosas.Mensaje = mes

                RemoveHandler botonBuscar.Click, AddressOf HumbleChoice.GenerarJuegos
                AddHandler botonBuscar.Click, AddressOf HumbleChoice.GenerarJuegos

                fechaDefecto = fechaDefecto.AddMonths(1)
                fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, 1)
            ElseIf cbTiendas.SelectedIndex = 2 Then
                botonBuscar.Visibility = Visibility.Visible
                tbIDs.Visibility = Visibility.Visible
                tbIDs.Text = String.Empty

                imagenTienda1.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/primegaming.png"
                imagenTienda2.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/primegaming.png"

                cosas.Tienda = Tiendas.amazoncomT
                cosas.Enlace = "https://gaming.amazon.com/"
                cosas.Mensaje = "New Games Added"

                RemoveHandler botonBuscar.Click, AddressOf PrimeGaming.GenerarJuegos
                AddHandler botonBuscar.Click, AddressOf PrimeGaming.GenerarJuegos

                fechaDefecto = fechaDefecto.AddMonths(1)
                fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, 1)
            ElseIf cbTiendas.SelectedIndex = 3 Then
                botonBuscar.Visibility = Visibility.Visible
                tbIDs.Visibility = Visibility.Collapsed

                imagenTienda1.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/xboxgamepass.png"
                imagenTienda2.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/xboxgamepass.png"

                cosas.Tienda = Tiendas.microsoftstoreT
                cosas.Enlace = "https://tinyurl.com/pepexbox"
                cosas.Titulo = "Xbox Game Pass • New Games Added • " + cosas.Juegos
                cosas.Mensaje = "New Games Added"

                RemoveHandler botonBuscar.Click, AddressOf Xbox.BuscarJuegos
                AddHandler botonBuscar.Click, AddressOf Xbox.BuscarJuegos

                fechaDefecto = fechaDefecto.AddDays(30)
                fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, fechaDefecto.Day)
            ElseIf cbTiendas.SelectedIndex = 4 Then
                botonBuscar.Visibility = Visibility.Visible
                tbIDs.Visibility = Visibility.Collapsed

                imagenTienda1.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/eaplay.png"
                imagenTienda2.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/eaplay.png"

                cosas.Tienda = Tiendas.originT
                cosas.Titulo = "EA Play • New Games Added • " + cosas.Juegos
                cosas.Mensaje = "New Games Added"
                cosas.Enlace = "https://www.origin.com/store/ea-play"

                RemoveHandler botonBuscar.Click, AddressOf EAPlay.BuscarJuegos
                AddHandler botonBuscar.Click, AddressOf EAPlay.BuscarJuegos

                fechaDefecto = fechaDefecto.AddDays(7)
                fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, fechaDefecto.Day)
            ElseIf cbTiendas.SelectedIndex = 5 Then
                botonBuscar.Visibility = Visibility.Visible
                tbIDs.Visibility = Visibility.Collapsed

                imagenTienda1.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/eaplaypro.png"
                imagenTienda2.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/eaplaypro.png"

                cosas.Tienda = Tiendas.originT
                cosas.Titulo = "EA Play Pro • New Games Added • " + cosas.Juegos
                cosas.Mensaje = "New Games Added"
                cosas.Enlace = "https://www.origin.com/store/ea-play"

                RemoveHandler botonBuscar.Click, AddressOf EAPlayPro.BuscarJuegos
                AddHandler botonBuscar.Click, AddressOf EAPlayPro.BuscarJuegos

                fechaDefecto = fechaDefecto.AddDays(7)
                fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, fechaDefecto.Day)
            ElseIf cbTiendas.SelectedIndex = 6 Then
                botonBuscar.Visibility = Visibility.Visible
                tbIDs.Visibility = Visibility.Collapsed

                imagenTienda1.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/humbletrove.png"
                imagenTienda2.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/humbletrove.png"

                cosas.Tienda = Tiendas.humbleT
                cosas.Tienda.NombreMostrar = "Humble Bundle"

                cosas.Titulo = "Humble Trove • New Games Added • " + cosas.Juegos
                cosas.Mensaje = "New Games Added"

                RemoveHandler botonBuscar.Click, AddressOf HumbleTrove.BuscarJuegos
                AddHandler botonBuscar.Click, AddressOf HumbleTrove.BuscarJuegos

                fechaDefecto = fechaDefecto.AddDays(7)
                fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, fechaDefecto.Day)
            ElseIf cbTiendas.SelectedIndex = 7 Then
                botonBuscar.Visibility = Visibility.Visible
                tbIDs.Visibility = Visibility.Collapsed

                imagenTienda1.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/geforcenow3.png"
                imagenTienda2.Source = "https://pepeizqdeals.com/wp-content/uploads/2020/12/geforcenow3.png"

                'cosas.Tienda = "Geforce"
                'cosas.Icono = "https://pepeizqdeals.com/wp-content/uploads/2020/03/tienda_geforcenow.jpg"
                cosas.Mensaje = "New Games Supported"

                RemoveHandler botonBuscar.Click, AddressOf GeforceNow.BuscarJuegos
                AddHandler botonBuscar.Click, AddressOf GeforceNow.BuscarJuegos

                fechaDefecto = fechaDefecto.AddDays(7)
                fechaPicker.SelectedDate = New DateTime(fechaDefecto.Year, fechaDefecto.Month, fechaDefecto.Day)
            End If

            If Not cosas.Titulo = Nothing Then
                tbTitulo.Text = Deals.LimpiarTitulo(cosas.Titulo)
            End If

            Dim panelMensaje As DropShadowPanel = pagina.FindName("panelMensajeTiendaEditorpepeizqdealsGenerarImagenSuscripcionesv2")
            Dim mensaje2 As TextBlock = pagina.FindName("mensajeTiendaEditorpepeizqdealsGenerarImagenSuscripcionesv2")

            If Not cosas.Mensaje = Nothing Then
                panelMensaje.Visibility = Visibility.Visible
                mensaje2.Text = cosas.Mensaje
            Else
                panelMensaje.Visibility = Visibility.Collapsed
                mensaje2.Text = String.Empty
            End If

            tbTitulo.Tag = cosas

            BloquearControles(True)

        End Sub

        Private Async Sub GenerarDatos2(sender As Object, e As RoutedEventArgs)

            BloquearControles(False)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbTitulo As TextBox = pagina.FindName("tbEditorTitulopepeizqdealsSubscriptions")
            Dim tbEnlace As TextBox = pagina.FindName("tbEditorEnlacepepeizqdealsSubscriptions")
            Dim tbJuegos As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsJuegos")

            Dim botonImagen As Button = pagina.FindName("botonEditorpepeizqdealsGenerarImagenSubscriptionsv2")
            Dim imagenTienda As ImageEx = pagina.FindName("imagenTiendaEditorpepeizqdealsGenerarImagenSuscripcionesv3")

            Dim cosas As Clases.Suscripciones = tbTitulo.Tag
            cosas.Tienda.LogoWebServidorEnlace300x80 = imagenTienda.Source

            Dim fechaPicker As DatePicker = pagina.FindName("fechaEditorpepeizqdealsSubscriptions")
            Dim horaPicker As TimePicker = pagina.FindName("horaEditorpepeizqdealsSubscriptions")

            Dim fechaFinal As DateTime = fechaPicker.SelectedDate.Value.Date
            fechaFinal = fechaFinal.AddHours(horaPicker.SelectedTime.Value.Hours)

            Dim tbImagenesGrid As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsEnlacesImagenGrid")
            Dim json As String = String.Empty

            If tbImagenesGrid.Text.Length > 0 Then
                json = DealsFormato.GenerarJsonSuscripciones(tbImagenesGrid.Text.Trim.Replace("header", "library_600x900"))
            End If

            Await Posts.Enviar(tbTitulo.Text.Trim, Nothing, 13, New List(Of Integer) From {9999}, cosas.Tienda,
                               cosas.Enlace, botonImagen, tbJuegos.Text.Trim, fechaFinal.ToString, Nothing, json, Nothing)

            BloquearControles(True)

        End Sub

        Private Sub LimpiarTexto(sender As Object, e As TextChangedEventArgs)

            Dim tb As TextBox = sender

            tb.Text = tb.Text.Replace("https://", Nothing)
            tb.Text = tb.Text.Replace("http://", Nothing)
            tb.Text = tb.Text.Replace("store.steampowered.com/app/", Nothing)
            tb.Text = tb.Text.Replace("steamdb.info/app/", Nothing)
            tb.Text = tb.Text.Replace("?curator_clanid=33500256", Nothing)
            tb.Text = tb.Text.Replace("/", Nothing)

        End Sub

        Private Sub GenerarImagenesJuegos(sender As Object, e As TextChangedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbImagenesGrid As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsEnlacesImagenGrid")
            Dim enlaces As String = tbImagenesGrid.Text.Trim
            Dim listaEnlaces As New List(Of String)

            Dim i As Integer = 0
            While i < 100
                If enlaces.Trim.Length > 0 Then
                    Dim enlace As String = String.Empty

                    If enlaces.Contains(",") Then
                        Dim int As Integer = enlaces.IndexOf(",")
                        enlace = enlaces.Remove(int, enlaces.Length - int)

                        enlaces = enlaces.Remove(0, int + 1)
                    Else
                        enlace = enlaces
                        enlaces = String.Empty
                    End If

                    enlace = enlace.Trim
                    listaEnlaces.Add(enlace)
                End If
                i += 1
            End While

            If listaEnlaces.Count > 0 Then
                If listaEnlaces.Count = 1 Then
                    ImagenEntrada.UnJuegoGenerar(listaEnlaces(0))
                Else
                    ImagenEntrada.DosJuegosGenerar(listaEnlaces)
                End If
            End If

        End Sub

        Private Sub CambioFechaAviso(sender As Object, e As DatePickerSelectedValueChangedEventArgs)

            Dim fechaPicker As DatePicker = sender

            If fechaPicker.SelectedDate.Value.Month = DateTime.Today.Month And fechaPicker.SelectedDate.Value.Day = DateTime.Today.Day Then
                Notificaciones.Toast("Hoy es el mismo dia", Nothing)
            End If

        End Sub

        Private Sub CopiarHtml(sender As Object, e As RoutedEventArgs)

            Dim boton As Button = sender
            Dim html As String = boton.Tag

            If html.Trim.Length > 0 Then
                Dim datos As New DataPackage With {
                    .RequestedOperation = DataPackageOperation.Copy
                }
                datos.SetText(html)
                Clipboard.SetContent(datos)
            End If

        End Sub

        Public Sub BloquearControles(estado As Boolean)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim cbTiendas As ComboBox = pagina.FindName("cbEditorpepeizqdealsSubscriptionsTiendas")
            cbTiendas.IsEnabled = estado

            Dim tbTitulo As TextBox = pagina.FindName("tbEditorTitulopepeizqdealsSubscriptions")
            tbTitulo.IsEnabled = estado

            Dim tbJuegos As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsJuegos")
            tbJuegos.IsEnabled = estado

            Dim tbImagenesGrid As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsEnlacesImagenGrid")
            tbImagenesGrid.IsEnabled = estado

            Dim tbImagenFondo As TextBox = pagina.FindName("tbEditorpepeizqdealsSuscripcionesImagenFondoUnaSuscripcion")
            tbImagenFondo.IsEnabled = estado

            Dim botonBuscar As Button = pagina.FindName("botonEditorpepeizqdealsSubscriptionsBuscar")
            botonBuscar.IsEnabled = estado

            Dim tbIDs As TextBox = pagina.FindName("tbEditorpepeizqdealsSubscriptionsIDs")
            tbIDs.IsEnabled = estado

            Dim fechaPicker As DatePicker = pagina.FindName("fechaEditorpepeizqdealsSubscriptions")
            fechaPicker.IsEnabled = estado

            Dim horaPicker As TimePicker = pagina.FindName("horaEditorpepeizqdealsSubscriptions")
            horaPicker.IsEnabled = estado

            Dim botonCopiarHtml As Button = pagina.FindName("botonEditorCopiarHtmlpepeizqdealsSubscriptions")
            botonCopiarHtml.IsEnabled = estado

            Dim botonSubir As Button = pagina.FindName("botonEditorSubirpepeizqdealsSubscriptions")
            botonSubir.IsEnabled = estado

        End Sub

    End Module

End Namespace

