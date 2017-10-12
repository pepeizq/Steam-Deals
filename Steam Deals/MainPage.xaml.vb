﻿Imports Microsoft.Services.Store.Engagement
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Windows.ApplicationModel.Core
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.Storage
Imports Windows.System
Imports Windows.UI

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Deals"), New SymbolIcon(Symbol.Home), 0))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Editor"), New SymbolIcon(Symbol.Edit), 1))
        nvPrincipal.MenuItems.Add(New NavigationViewItemSeparator)
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Config"), New SymbolIcon(Symbol.Setting), 2))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("MoreThings"), New SymbolIcon(Symbol.More), 3))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        Dim item As TextBlock = args.InvokedItem

        If item.Text = recursos.GetString("Deals") Then
            GridVisibilidad(gridDeals, item.Text)
        ElseIf item.Text = recursos.GetString("Editor") Then
            GridVisibilidad(gridEditor, item.Text)
        ElseIf item.Text = recursos.GetString("Config") Then
            GridVisibilidad(gridConfig, item.Text)
        ElseIf item.Text = recursos.GetString("MoreThings") Then
            GridVisibilidad(gridMasCosas, item.Text)
            NavegarMasCosas(lvMasCosasMasApps, "https://pepeizqapps.com/")
        End If

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        Dim coreBarra As CoreApplicationViewTitleBar = CoreApplication.GetCurrentView.TitleBar
        coreBarra.ExtendViewIntoTitleBar = True

        Dim barra As ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar
        barra.ButtonBackgroundColor = Colors.Transparent
        barra.ButtonForegroundColor = Colors.White
        barra.ButtonInactiveBackgroundColor = Colors.Transparent

        '--------------------------------------------------------

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        GridVisibilidad(gridDeals, recursos.GetString("Deals"))
        nvPrincipal.IsPaneOpen = False

        If ApplicationData.Current.LocalSettings.Values("ordenar") = Nothing Then
            cbConfigTipoOrdenar.SelectedIndex = 0
            ApplicationData.Current.LocalSettings.Values("ordenar") = 0
        Else
            cbConfigTipoOrdenar.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        End If

        '--------------------------------------------------------

        cbOrdenarSteam.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGamersGate.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGamesPlanet.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarHumble.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGreenManGaming.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarBundleStars.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGOG.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarWinGameStore.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarSilaGames.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarNuuvem.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarMicrosoftStore.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarAmazonEs.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarAmazonUk.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")

        '--------------------------------------------------------

        If Not ApplicationData.Current.LocalSettings.Values("cuentasteam") = Nothing Then
            tbSteamConfigCuentaID.Text = ApplicationData.Current.LocalSettings.Values("cuentasteam")
        End If

        If Not ApplicationData.Current.LocalSettings.Values("descartarjuegos") = Nothing Then
            If ApplicationData.Current.LocalSettings.Values("descartarjuegos") = "on" Then
                cbConfigDescartarMisJuegos.IsChecked = True
                spSteamConfigCuenta.Visibility = Visibility.Visible
            Else
                cbConfigDescartarMisJuegos.IsChecked = False
                spSteamConfigCuenta.Visibility = Visibility.Collapsed
            End If
        Else
            ApplicationData.Current.LocalSettings.Values("descartarjuegos") = "off"
            cbConfigDescartarMisJuegos.IsChecked = False
            spSteamConfigCuenta.Visibility = Visibility.Collapsed
        End If

        If Not ApplicationData.Current.LocalSettings.Values("descartarjuegosultimavisita") = Nothing Then
            If ApplicationData.Current.LocalSettings.Values("descartarjuegosultimavisita") = "on" Then
                cbConfigDescartarUltimaVisita.IsChecked = True
            Else
                cbConfigDescartarUltimaVisita.IsChecked = False
            End If
        Else
            ApplicationData.Current.LocalSettings.Values("descartarjuegos") = "on"
            cbConfigDescartarUltimaVisita.IsChecked = True
        End If

        If ApplicationData.Current.LocalSettings.Values("editor") = Nothing Then
            cbConfigEditor.IsChecked = False
            EditorVisibilidad(False)
            ApplicationData.Current.LocalSettings.Values("editor") = "off"
        Else
            If ApplicationData.Current.LocalSettings.Values("editor") = "on" Then
                cbConfigEditor.IsChecked = True
                EditorVisibilidad(True)
            Else
                cbConfigEditor.IsChecked = False
                EditorVisibilidad(False)
            End If
        End If

        '--------------------------------------------------------

        If ApplicationData.Current.LocalSettings.Values("editorTipo") = Nothing Then
            cbEditorTipo.SelectedIndex = 0
            ApplicationData.Current.LocalSettings.Values("editorTipo") = 0
        Else
            cbEditorTipo.SelectedIndex = ApplicationData.Current.LocalSettings.Values("editorTipo")
        End If

        Editor.Borrar()
        Divisas.Generar()

    End Sub

    Private Sub GridVisibilidad(grid As Grid, tag As String)

        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + tag

        gridDeals.Visibility = Visibility.Collapsed
        gridEditor.Visibility = Visibility.Collapsed
        gridConfig.Visibility = Visibility.Collapsed
        gridMasCosas.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    Private Sub LvTiendasItemClick(sender As Object, args As ItemClickEventArgs)

        If panelMensajeTienda.Visibility = Visibility.Visible Then
            panelMensajeTienda.Visibility = Visibility.Collapsed
        End If

        botonTiendaSteam.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaGamersGate.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaGamesPlanet.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaHumble.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaGreenManGaming.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaBundleStars.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaGOG.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaWinGameStore.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaSilaGames.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaNuuvem.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaMicrosoftStore.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaAmazonEs.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        botonTiendaAmazonUk.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))

        gridTiendaSteam.Visibility = Visibility.Collapsed
        gridTiendaGamersGate.Visibility = Visibility.Collapsed
        gridTiendaGamesPlanet.Visibility = Visibility.Collapsed
        gridTiendaHumble.Visibility = Visibility.Collapsed
        gridTiendaGreenManGaming.Visibility = Visibility.Collapsed
        gridTiendaBundleStars.Visibility = Visibility.Collapsed
        gridTiendaGOG.Visibility = Visibility.Collapsed
        gridTiendaWinGameStore.Visibility = Visibility.Collapsed
        gridTiendaSilaGames.Visibility = Visibility.Collapsed
        gridTiendaNuuvem.Visibility = Visibility.Collapsed
        gridTiendaMicrosoftStore.Visibility = Visibility.Collapsed
        gridTiendaAmazonEs.Visibility = Visibility.Collapsed
        gridTiendaAmazonUk.Visibility = Visibility.Collapsed

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Steam"
            gridTiendaSteam.Visibility = Visibility.Visible
            botonTiendaSteam.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoSteam.Items.Count = 0 Then
                If gridProgresoSteam.Visibility = Visibility.Collapsed Then
                    Steam.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - GamersGate"
            gridTiendaGamersGate.Visibility = Visibility.Visible
            botonTiendaGamersGate.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoGamersGate.Items.Count = 0 Then
                If gridProgresoGamersGate.Visibility = Visibility.Collapsed Then
                    GamersGate.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 2 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - GamesPlanet"
            gridTiendaGamesPlanet.Visibility = Visibility.Visible
            botonTiendaGamesPlanet.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoGamesPlanet.Items.Count = 0 Then
                If gridProgresoGamesPlanet.Visibility = Visibility.Collapsed Then
                    GamesPlanet.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 3 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Humble Store"
            gridTiendaHumble.Visibility = Visibility.Visible
            botonTiendaHumble.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoHumble.Items.Count = 0 Then
                If gridProgresoHumble.Visibility = Visibility.Collapsed Then
                    Humble.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 4 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Green Man Gaming"
            gridTiendaGreenManGaming.Visibility = Visibility.Visible
            botonTiendaGreenManGaming.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoGreenManGaming.Items.Count = 0 Then
                If gridProgresoGreenManGaming.Visibility = Visibility.Collapsed Then
                    GreenManGaming.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 5 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Bundle Stars"
            gridTiendaBundleStars.Visibility = Visibility.Visible
            botonTiendaBundleStars.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoBundleStars.Items.Count = 0 Then
                If gridProgresoBundleStars.Visibility = Visibility.Collapsed Then
                    BundleStars.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 6 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - GOG"
            gridTiendaGOG.Visibility = Visibility.Visible
            botonTiendaGOG.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoGOG.Items.Count = 0 Then
                If gridProgresoGOG.Visibility = Visibility.Collapsed Then
                    GOG.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 7 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - WinGameStore"
            gridTiendaWinGameStore.Visibility = Visibility.Visible
            botonTiendaWinGameStore.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoWinGameStore.Items.Count = 0 Then
                If gridProgresoWinGameStore.Visibility = Visibility.Collapsed Then
                    WinGameStore.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 8 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Sila Games"
            gridTiendaSilaGames.Visibility = Visibility.Visible
            botonTiendaSilaGames.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoSilaGames.Items.Count = 0 Then
                If gridProgresoSilaGames.Visibility = Visibility.Collapsed Then
                    SilaGames.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 9 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Nuuvem"
            gridTiendaNuuvem.Visibility = Visibility.Visible
            botonTiendaNuuvem.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoNuuvem.Items.Count = 0 Then
                If gridProgresoNuuvem.Visibility = Visibility.Collapsed Then
                    Nuuvem.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 10 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Microsoft Store"
            gridTiendaMicrosoftStore.Visibility = Visibility.Visible
            botonTiendaMicrosoftStore.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoMicrosoftStore.Items.Count = 0 Then
                If gridProgresoMicrosoftStore.Visibility = Visibility.Collapsed Then
                    MicrosoftStore.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 11 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Amazon.es"
            gridTiendaAmazonEs.Visibility = Visibility.Visible
            botonTiendaAmazonEs.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoAmazonEs.Items.Count = 0 Then
                If gridProgresoAmazonEs.Visibility = Visibility.Collapsed Then
                    AmazonEs.GenerarOfertas()
                End If
            End If

        ElseIf sp.Tag.ToString = 12 Then

            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - Amazon.co.uk"
            gridTiendaAmazonUk.Visibility = Visibility.Visible
            botonTiendaAmazonUk.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

            If listadoAmazonUk.Items.Count = 0 Then
                If gridProgresoAmazonUk.Visibility = Visibility.Collapsed Then
                    AmazonUk.GenerarOfertas()
                End If
            End If

        End If

    End Sub

    'OFERTAS-----------------------------------------------------------------------------

    Private Async Sub ListadoClick(grid As Grid)

        Try
            If ApplicationData.Current.LocalSettings.Values("editor") = "off" Then
                Dim juego As Juego = grid.Tag
                Dim enlace As String = Nothing

                If Not juego.Afiliado1 = Nothing Then
                    enlace = juego.Afiliado1
                Else
                    enlace = juego.Enlace1
                End If

                Await Launcher.LaunchUriAsync(New Uri(enlace))
            Else
                Dim cb As CheckBox = grid.Children.Item(grid.Children.Count - 1)

                If cb.IsChecked = True Then
                    cb.IsChecked = False
                Else
                    cb.IsChecked = True
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    'OFERTAS-STEAM----------------------------------------------------------------------------

    Private Sub ListadoSteam_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoSteam.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesSteamItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoSteam, "Steam")

        ElseIf sp.Tag.ToString = 1 Then

            Steam.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorSteamItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaSteam.Visibility = Visibility.Visible Then
                If Not gridProgresoSteam.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("Steam", cbOrdenarSteam.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoSteam, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoSteam, False)

        End If

    End Sub

    Private Sub CbOrdenarSteam_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarSteam.SelectionChanged

        If gridTiendaSteam.Visibility = Visibility.Visible Then
            If Not gridProgresoSteam.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("Steam", cbOrdenarSteam.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-GAMERSGATE----------------------------------------------------------------------------

    Private Sub ListadoGamersGate_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoGamersGate.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesGamersGateItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoGamersGate, "GamersGate")

        ElseIf sp.Tag.ToString = 1 Then

            GamersGate.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorGamersGateItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaGamersGate.Visibility = Visibility.Visible Then
                If Not gridProgresoGamersGate.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("GamersGate", cbOrdenarGamersGate.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoGamersGate, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoGamersGate, False)

        End If

    End Sub

    Private Sub CbOrdenarGamersGate_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarGamersGate.SelectionChanged

        If gridTiendaGamersGate.Visibility = Visibility.Visible Then
            If Not gridProgresoGamersGate.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("GamersGate", cbOrdenarGamersGate.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-GAMESPLANET----------------------------------------------------------------------------

    Private Sub ListadoGamesPlanet_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoGamesPlanet.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesGamesPlanetItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoGamesPlanet, "GamesPlanet")

        ElseIf sp.Tag.ToString = 1 Then

            GamesPlanet.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorGamesPlanetItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaGamesPlanet.Visibility = Visibility.Visible Then
                If Not gridProgresoGamesPlanet.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("GamesPlanet", cbOrdenarGamesPlanet.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoGamesPlanet, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoGamesPlanet, False)

        End If

    End Sub

    Private Sub CbOrdenarGamesPlanet_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarGamesPlanet.SelectionChanged

        If gridTiendaGamesPlanet.Visibility = Visibility.Visible Then
            If Not gridProgresoGamesPlanet.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("GamesPlanet", cbOrdenarGamesPlanet.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-HUMBLE----------------------------------------------------------------------------

    Private Sub ListadoHumble_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoHumble.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesHumbleItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoHumble, "HumbleStore")

        ElseIf sp.Tag.ToString = 1 Then

            Humble.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorHumbleItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaHumble.Visibility = Visibility.Visible Then
                If Not gridProgresoHumble.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("Humble", cbOrdenarHumble.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoHumble, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoHumble, False)

        End If

    End Sub

    Private Sub CbOrdenarHumble_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarHumble.SelectionChanged

        If gridTiendaHumble.Visibility = Visibility.Visible Then
            If Not gridProgresoHumble.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("Humble", cbOrdenarHumble.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-GREENMANGAMING----------------------------------------------------------------------------

    Private Sub ListadoGreenManGaming_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoGreenManGaming.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesGreenManGamingItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoGreenManGaming, "GreenManGaming")

        ElseIf sp.Tag.ToString = 1 Then

            GreenManGaming.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorGreenManGamingItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaGreenManGaming.Visibility = Visibility.Visible Then
                If Not gridProgresoGreenManGaming.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("GreenManGaming", cbOrdenarGreenManGaming.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoGreenManGaming, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoGreenManGaming, False)

        End If

    End Sub

    Private Sub CbOrdenarGreenManGaming_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarGreenManGaming.SelectionChanged

        If gridTiendaGreenManGaming.Visibility = Visibility.Visible Then
            If Not gridProgresoGreenManGaming.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("GreenManGaming", cbOrdenarGreenManGaming.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-BUNDLESTARS----------------------------------------------------------------------------

    Private Sub ListadoBundleStars_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoBundleStars.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesBundleStarsItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoBundleStars, "BundleStars")

        ElseIf sp.Tag.ToString = 1 Then

            BundleStars.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorBundleStarsItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaBundleStars.Visibility = Visibility.Visible Then
                If Not gridProgresoBundleStars.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("BundleStars", cbOrdenarBundleStars.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoBundleStars, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoBundleStars, False)

        End If

    End Sub

    Private Sub CbOrdenarBundleStars_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarBundleStars.SelectionChanged

        If gridTiendaBundleStars.Visibility = Visibility.Visible Then
            If Not gridProgresoBundleStars.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("BundleStars", cbOrdenarBundleStars.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-GOG----------------------------------------------------------------------------

    Private Sub ListadoGOG_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoGOG.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesGOGItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoGOG, "GOG")

        ElseIf sp.Tag.ToString = 1 Then

            GOG.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorGOGItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaGOG.Visibility = Visibility.Visible Then
                If Not gridProgresoGOG.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("GOG", cbOrdenarGOG.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoGOG, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoGOG, False)

        End If

    End Sub

    Private Sub CbOrdenarGOG_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarGOG.SelectionChanged

        If gridTiendaGOG.Visibility = Visibility.Visible Then
            If Not gridProgresoGOG.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("GOG", cbOrdenarGOG.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-WINGAMESTORE----------------------------------------------------------------------------

    Private Sub ListadoWinGameStore_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoWinGameStore.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesWinGameStoreItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoWinGameStore, "WinGameStore")

        ElseIf sp.Tag.ToString = 1 Then

            WinGameStore.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorWinGameStoreItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaWinGameStore.Visibility = Visibility.Visible Then
                If Not gridProgresoWinGameStore.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("WinGameStore", cbOrdenarWinGameStore.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoWinGameStore, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoWinGameStore, False)

        End If

    End Sub

    Private Sub CbOrdenarWinGameStore_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarWinGameStore.SelectionChanged

        If gridTiendaWinGameStore.Visibility = Visibility.Visible Then
            If Not gridProgresoWinGameStore.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("WinGameStore", cbOrdenarWinGameStore.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-SILAGAMES----------------------------------------------------------------------------

    Private Sub ListadoSilaGames_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoSilaGames.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesSilaGamesItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoSilaGames, "SilaGames")

        ElseIf sp.Tag.ToString = 1 Then

            SilaGames.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorSilaGamesItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaSilaGames.Visibility = Visibility.Visible Then
                If Not gridProgresoSilaGames.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("SilaGames", cbOrdenarSilaGames.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoSilaGames, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoSilaGames, False)

        End If

    End Sub

    Private Sub CbOrdenarSilaGames_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarSilaGames.SelectionChanged

        If gridTiendaSilaGames.Visibility = Visibility.Visible Then
            If Not gridProgresoSilaGames.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("SilaGames", cbOrdenarSilaGames.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-NUUVEM----------------------------------------------------------------------------

    Private Sub ListadoNuuvem_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoNuuvem.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesNuuvemItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoNuuvem, "Nuuvem")

        ElseIf sp.Tag.ToString = 1 Then

            Nuuvem.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorNuuvemItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaNuuvem.Visibility = Visibility.Visible Then
                If Not gridProgresoNuuvem.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("Nuuvem", cbOrdenarNuuvem.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoNuuvem, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoNuuvem, False)

        End If

    End Sub

    Private Sub CbOrdenarNuuvem_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarNuuvem.SelectionChanged

        If gridTiendaNuuvem.Visibility = Visibility.Visible Then
            If Not gridProgresoNuuvem.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("Nuuvem", cbOrdenarNuuvem.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-MICROSOFTSTORE----------------------------------------------------------------------------

    Private Sub ListadoMicrosoftStore_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoMicrosoftStore.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesMicrosoftStoreItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoMicrosoftStore, "MicrosoftStore")

        ElseIf sp.Tag.ToString = 1 Then

            MicrosoftStore.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorMicrosoftStoreItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaMicrosoftStore.Visibility = Visibility.Visible Then
                If Not gridProgresoMicrosoftStore.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("MicrosoftStore", cbOrdenarMicrosoftStore.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoMicrosoftStore, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoMicrosoftStore, False)

        End If

    End Sub

    Private Sub CbOrdenarMicrosoftStore_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarMicrosoftStore.SelectionChanged

        If gridTiendaMicrosoftStore.Visibility = Visibility.Visible Then
            If Not gridProgresoMicrosoftStore.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("MicrosoftStore", cbOrdenarMicrosoftStore.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-AMAZONES----------------------------------------------------------------------------

    Private Sub ListadoAmazonEs_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoAmazonEs.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesAmazonEsItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoAmazonEs, "AmazonEs")

        ElseIf sp.Tag.ToString = 1 Then

            AmazonEs.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorAmazonEsItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaAmazonEs.Visibility = Visibility.Visible Then
                If Not gridProgresoAmazonEs.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("AmazonEs", cbOrdenarAmazonEs.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoAmazonEs, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoAmazonEs, False)

        End If

    End Sub

    Private Sub CbOrdenarAmazonEs_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarAmazonEs.SelectionChanged

        If gridTiendaAmazonEs.Visibility = Visibility.Visible Then
            If Not gridProgresoAmazonEs.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("AmazonEs", cbOrdenarAmazonEs.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'OFERTAS-AMAZONUK----------------------------------------------------------------------------

    Private Sub ListadoAmazonUk_ItemClick(sender As Object, e As ItemClickEventArgs) Handles listadoAmazonUk.ItemClick

        ListadoClick(e.ClickedItem)

    End Sub

    Private Sub LvOpcionesAmazonUkItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Captura.Generar(listadoAmazonUk, "AmazonUk")

        ElseIf sp.Tag.ToString = 1 Then

            AmazonUk.GenerarOfertas()

        End If

    End Sub

    Private Sub LvEditorAmazonUkItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            If gridTiendaAmazonUk.Visibility = Visibility.Visible Then
                If Not gridProgresoAmazonUk.Visibility = Visibility.Visible Then
                    Ordenar.Ofertas("AmazonUk", cbOrdenarAmazonUk.SelectedIndex, False, True)
                End If
            End If

        ElseIf sp.Tag.ToString = 1 Then

            SeleccionarEnlaces(listadoAmazonUk, True)

        ElseIf sp.Tag.ToString = 2 Then

            SeleccionarEnlaces(listadoAmazonUk, False)

        End If

    End Sub

    Private Sub CbOrdenarAmazonUk_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbOrdenarAmazonUk.SelectionChanged

        If gridTiendaAmazonUk.Visibility = Visibility.Visible Then
            If Not gridProgresoAmazonUk.Visibility = Visibility.Visible Then
                Ordenar.Ofertas("AmazonUk", cbOrdenarAmazonUk.SelectedIndex, False, False)
            End If
        End If

    End Sub

    'CONFIG-----------------------------------------------------------------------------

    Private Sub TbSteamConfigCuentaID_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbSteamConfigCuentaID.TextChanged

        CuentaSteam.BuscarJuegos()

    End Sub

    Private Sub CbConfigDescartarMisJuegos_Checked(sender As Object, e As RoutedEventArgs) Handles cbConfigDescartarMisJuegos.Checked

        ApplicationData.Current.LocalSettings.Values("descartarjuegos") = "on"
        spSteamConfigCuenta.Visibility = Visibility.Visible

        If cbConfigDescartarUltimaVisita.IsChecked = True Then
            ApplicationData.Current.LocalSettings.Values("descartarjuegosultimavisita") = "off"
            cbConfigDescartarUltimaVisita.IsChecked = False
        End If

    End Sub

    Private Sub CbConfigDescartarMisJuegos_Unchecked(sender As Object, e As RoutedEventArgs) Handles cbConfigDescartarMisJuegos.Unchecked

        ApplicationData.Current.LocalSettings.Values("descartarjuegos") = "off"
        spSteamConfigCuenta.Visibility = Visibility.Collapsed

    End Sub

    Private Sub CbConfigDescartarUltimaVisita_Checked(sender As Object, e As RoutedEventArgs) Handles cbConfigDescartarUltimaVisita.Checked

        ApplicationData.Current.LocalSettings.Values("descartarjuegosultimavisita") = "on"

        If cbConfigDescartarMisJuegos.IsChecked = True Then
            ApplicationData.Current.LocalSettings.Values("descartarjuegos") = "off"
            cbConfigDescartarMisJuegos.IsChecked = False
        End If

    End Sub

    Private Sub CbConfigDescartarUltimaVisita_Unchecked(sender As Object, e As RoutedEventArgs) Handles cbConfigDescartarUltimaVisita.Unchecked

        ApplicationData.Current.LocalSettings.Values("descartarjuegosultimavisita") = "off"

    End Sub

    Private Sub CbConfigTipoOrdenar_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbConfigTipoOrdenar.SelectionChanged

        ApplicationData.Current.LocalSettings.Values("ordenar") = cbConfigTipoOrdenar.SelectedIndex

        cbOrdenarSteam.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGamersGate.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGamesPlanet.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarHumble.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGreenManGaming.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarBundleStars.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarGOG.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarWinGameStore.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarSilaGames.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarNuuvem.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarMicrosoftStore.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarAmazonEs.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")
        cbOrdenarAmazonUk.SelectedIndex = ApplicationData.Current.LocalSettings.Values("ordenar")

    End Sub

    'EDITOR-----------------------------------------

    Private Sub CbConfigEditor_Checked(sender As Object, e As RoutedEventArgs) Handles cbConfigEditor.Checked

        ApplicationData.Current.LocalSettings.Values("editor") = "on"
        EditorVisibilidad(True)

    End Sub

    Private Sub CbConfigEditor_Unchecked(sender As Object, e As RoutedEventArgs) Handles cbConfigEditor.Unchecked

        ApplicationData.Current.LocalSettings.Values("editor") = "off"
        EditorVisibilidad(False)

    End Sub

    Private Sub EditorVisibilidad(estado As Boolean)

        If estado = True Then
            For Each item In nvPrincipal.MenuItems
                Dim menuItem As NavigationViewItem = Nothing

                Try
                    menuItem = item
                Catch ex As Exception

                End Try

                If Not menuItem Is Nothing Then
                    If menuItem.Tag = 1 Then
                        menuItem.Visibility = Visibility.Visible
                    End If
                End If
            Next

            botonTiendaAmazonEs.Visibility = Visibility.Visible
            botonTiendaAmazonUk.Visibility = Visibility.Visible

            spEditorSteam.Visibility = Visibility.Visible
            spEditorGamersGate.Visibility = Visibility.Visible
            spEditorGamesPlanet.Visibility = Visibility.Visible
            spEditorHumble.Visibility = Visibility.Visible
            spEditorGreenManGaming.Visibility = Visibility.Visible
            spEditorBundleStars.Visibility = Visibility.Visible
            spEditorGOG.Visibility = Visibility.Visible
            spEditorWinGameStore.Visibility = Visibility.Visible
            spEditorSilaGames.Visibility = Visibility.Visible
            spEditorNuuvem.Visibility = Visibility.Visible
            spEditorMicrosoftStore.Visibility = Visibility.Visible
            spEditorAmazonEs.Visibility = Visibility.Visible
            spEditorAmazonUk.Visibility = Visibility.Visible
        Else
            For Each item In nvPrincipal.MenuItems
                Dim menuItem As NavigationViewItem = Nothing

                Try
                    menuItem = item
                Catch ex As Exception

                End Try

                If Not menuItem Is Nothing Then
                    If menuItem.Tag = 1 Then
                        menuItem.Visibility = Visibility.Collapsed
                    End If
                End If
            Next

            botonTiendaAmazonEs.Visibility = Visibility.Collapsed
            botonTiendaAmazonUk.Visibility = Visibility.Collapsed

            spEditorSteam.Visibility = Visibility.Collapsed
            spEditorGamersGate.Visibility = Visibility.Collapsed
            spEditorGamesPlanet.Visibility = Visibility.Collapsed
            spEditorHumble.Visibility = Visibility.Collapsed
            spEditorGreenManGaming.Visibility = Visibility.Collapsed
            spEditorBundleStars.Visibility = Visibility.Collapsed
            spEditorGOG.Visibility = Visibility.Collapsed
            spEditorWinGameStore.Visibility = Visibility.Collapsed
            spEditorSilaGames.Visibility = Visibility.Collapsed
            spEditorNuuvem.Visibility = Visibility.Collapsed
            spEditorMicrosoftStore.Visibility = Visibility.Collapsed
            spEditorAmazonEs.Visibility = Visibility.Collapsed
            spEditorAmazonUk.Visibility = Visibility.Collapsed
        End If

    End Sub

    Private Sub TbEditorEnlaces_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbEditorEnlaces.TextChanged

        If tbEditorEnlaces.Visibility = Visibility.Visible Then
            tbEditorNumCaracteres.Text = tbEditorEnlaces.Text.Length.ToString
        End If

    End Sub

    Private Sub CbEditorTipo_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbEditorTipo.SelectionChanged

        ApplicationData.Current.LocalSettings.Values("editorTipo") = cbEditorTipo.SelectedIndex
        Editor.Generar()
        Editor.GenerarOpciones()

    End Sub

    Private Async Sub LvEditorItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Dim datos As DataPackage = New DataPackage
            datos.SetText(tbEditorTitulo.Text)
            Clipboard.SetContent(datos)

        ElseIf sp.Tag.ToString = 1 Then

            Dim datos As DataPackage = New DataPackage
            datos.SetText(tbEditorTitulo.Text)
            Clipboard.SetContent(datos)
            tbEditorTitulo.Text = String.Empty

        ElseIf sp.Tag.ToString = 2 Then

            Try
                Dim contenidoEnlaces As String = Nothing

                If Not tbEditorEnlaces.Text = Nothing Then
                    contenidoEnlaces = tbEditorEnlaces.Text
                Else
                    Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
                    contenidoEnlaces = Await helper.ReadFileAsync(Of String)("contenidoEnlaces")
                End If

                Dim datos As DataPackage = New DataPackage
                datos.SetText(contenidoEnlaces)
                Clipboard.SetContent(datos)
            Catch ex As Exception

            End Try

        ElseIf sp.Tag.ToString = 3 Then

            Try
                Dim contenidoEnlaces As String = Nothing

                If Not tbEditorEnlaces.Text = Nothing Then
                    contenidoEnlaces = tbEditorEnlaces.Text
                Else
                    Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
                    contenidoEnlaces = Await helper.ReadFileAsync(Of String)("contenidoEnlaces")
                End If

                Dim datos As DataPackage = New DataPackage
                datos.SetText(contenidoEnlaces)
                Clipboard.SetContent(datos)
                tbEditorEnlaces.Text = String.Empty
            Catch ex As Exception

            End Try

        ElseIf sp.Tag.ToString = 4 Then

            Editor.Borrar()

        End If

    End Sub

    Private Sub BotonValoracionActualizar_Click(sender As Object, e As RoutedEventArgs) Handles botonValoracionActualizar.Click

        Valoracion.Generar()

    End Sub

    Private Async Sub SeleccionarEnlaces(listado As ListView, estado As Boolean)

        Dim listaGrids As ItemCollection = listado.Items

        For Each item In listaGrids
            Dim grid As Grid = item
            Dim cb As CheckBox = grid.Children.Item(grid.Children.Count - 1)
            Await Task.Delay(700)
            cb.IsChecked = estado
        Next

    End Sub

    'MASCOSAS-----------------------------------------

    Private Async Sub LvMasCosasItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))

        ElseIf sp.Tag.ToString = 1 Then

            NavegarMasCosas(lvMasCosasMasApps, "https://pepeizqapps.com/")

        ElseIf sp.Tag.ToString = 2 Then

            NavegarMasCosas(lvMasCosasActualizaciones, "https://pepeizqapps.com/updates/")

        ElseIf sp.Tag.ToString = 3 Then

            NavegarMasCosas(lvMasCosasContacto, "https://pepeizqapps.com/contact/")

        ElseIf sp.Tag.ToString = 4 Then

            If StoreServicesFeedbackLauncher.IsSupported = True Then
                Dim ejecutador As StoreServicesFeedbackLauncher = StoreServicesFeedbackLauncher.GetDefault()
                Await ejecutador.LaunchAsync()
            Else
                NavegarMasCosas(lvMasCosasReportarFallo, "https://pepeizqapps.com/contact/")
            End If

        ElseIf sp.Tag.ToString = 5 Then

            NavegarMasCosas(lvMasCosasTraduccion, "https://poeditor.com/join/project/YaZAR0uIW4")

        ElseIf sp.Tag.ToString = 6 Then

            NavegarMasCosas(lvMasCosasCodigoFuente, "https://github.com/pepeizq/Steam-Deals")

        End If

    End Sub

    Private Sub NavegarMasCosas(lvItem As ListViewItem, url As String)

        lvMasCosasMasApps.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasActualizaciones.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasContacto.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasReportarFallo.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasTraduccion.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasCodigoFuente.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))

        lvItem.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

        pbMasCosas.Visibility = Visibility.Visible

        wvMasCosas.Navigate(New Uri(url))

    End Sub

    Private Sub WvMasCosas_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles wvMasCosas.NavigationCompleted

        pbMasCosas.Visibility = Visibility.Collapsed

    End Sub

End Class
