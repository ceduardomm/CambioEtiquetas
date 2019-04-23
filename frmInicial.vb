Imports System.Data
Imports System.Data.SqlClient
Public Class frmInicial
    Public Pedido As Integer
    Private Sub frmInicial_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MdiParent = frmPrincipal
        Dim strSQL1 As String = "Select T0.ItemCode,T0.ItemName,T0.CodeBars,T1.Price From OITM T0 INNER JOIN ITM1 T1 ON T0.ItemCode=T1.ItemCode WHERE T1.PriceList =1"
        Dim strSQL2 As String = "Select CardCode,CardName,CardFName From OCRD Where CardCode Like" & "'" & "C%" & "'"
        Dim objComm As New SqlCommand(strSQL1, My.Forms.frmPrincipal.objConex)
        Dim objAdapter As New SqlDataAdapter
        objAdapter.SelectCommand = objComm
        My.Forms.frmPrincipal.miDataSet = New DataSet
        Try
            objAdapter.Fill(My.Forms.frmPrincipal.miDataSet, "Productos")
            objComm = New SqlCommand(strSQL2, My.Forms.frmPrincipal.objConex)
            objAdapter.SelectCommand = objComm
            objAdapter.Fill(My.Forms.frmPrincipal.miDataSet, "Clientes")
        Catch ex As InvalidOperationException
            MessageBox.Show("la conexion no ha sido iniciada, salga y vuelva a intentarlo")
        Catch ex As SqlException
            MessageBox.Show("Tiempo de espera caducó,problema de conexion")
        End Try
        'No habilito como origen ninguna tabla del DataSet porque yo quiero
        'formar las columnas a mi gusto y llenarlas 
        'Me.DataGridView1.DataSource = Me.miDataSet.Tables(1)
        'Con esta conexion logro leer el ultimo numero de pedido para continuar con el siguiente
        Dim objCon As SqlConnection
        Dim miComando As SqlCommand
        Dim sentenciaSQL As String = "select PedidoID from Etiquetas Order by PedidoID desc"
        Dim StringCon As String = "Data Source=AUXINFORMATICA;Initial Catalog=Etiquetas;Integrated Security=yes"
        Dim lector As Integer
        objCon = New SqlConnection(StringCon)
        Try
            objCon.Open()
            miComando = New SqlCommand(sentenciaSQL, objCon)
            lector = CInt(miComando.ExecuteScalar)
            If objCon.Equals(Nothing) Then
                Exit Sub
            Else
                If lector = 0 Then
                    Pedido = 1
                Else
                    Pedido = lector + 1
                End If
            End If
        Catch ex As Exception
        Finally
            objCon.Close()
        End Try
    End Sub

    'Acceder a la conexion de la base de datos local para obtener el numero de pedido
    'disponible para asignar.
    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        If (txtCustomerCode.Text = Nothing) Or (txtCustomerCode.TextLength < 7) Or (txtCustomerCode.TextLength > 7) Then
            MessageBox.Show("No puede quedar vacio el codigo del cliente, codigo esta imcompleto o Excedido", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Try
                Dim CusRows() As DataRow = My.Forms.frmPrincipal.miDataSet.Tables("Clientes").Select("CardCode='" & txtCustomerCode.Text.ToUpper & "'")
                If (CusRows.Length <= 0) Then
                    MessageBox.Show("No existe cliente")
                    txtCustomerCode.Focus()
                    Exit Sub
                End If
                Try
                    'en este espacio asigno a la caja de texto el nombre del cliente correpondiente
                    'al codigo digitado; simepre y cuando exista.
                    txtCusName.Text = CusRows(0).Item(1) & CusRows(0).Item(2)
                    txtCustomerCode.Text = ""
                    TxtFecha.Text = Date.Today
                    'Me.DataGridView1.Columns("Barra").ReadOnly = True
                    Me.DataGridView1.Columns("Descripcion").ReadOnly = True
                    Me.DataGridView1.Columns("Precio").ReadOnly = True
                    Me.DataGridView1.Enabled = True
                Catch ex As Exception
                    MessageBox.Show("El campo CardFName es nulo,asignele un valor por favor", ex.Message)
                End Try
            Catch ex As Exception
                MessageBox.Show("Hay problemas con la conexion de red,revise por favor o comuniquelo al Administrador de Red")
            End Try
            'Dim i As Integer
            'Este procedimiento es para llenar el Grid con las columnas de la tabla
            'si es que ya esta establecido como origen dicha tabla desde el DataSet.
            'i = 0
            'For Each row As DataRow In CusRows
            'Me.DataGridView1.Columns.Add(CusRows(i).Item(i))
            'i += 1
            'Next
        End If
    End Sub

    Private Sub DataGridView1_RowLeave(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.RowLeave
        Dim prodcod, prodcodbars As String
        Dim indice As Integer
        indice = e.RowIndex
        prodcod = Me.DataGridView1.Rows(indice).Cells.Item("Producto_ID").Value
        prodcodbars = Me.DataGridView1.Rows(indice).Cells.Item("Barra").Value
        Dim ProdRows() As DataRow
        Try
            ProdRows = My.Forms.frmPrincipal.miDataSet.Tables("Productos").Select("CodeBars ='" & prodcodbars & "'")
            Me.DataGridView1.Rows(indice).Cells.Item("Producto_ID").Value = ProdRows(0).Item(0)
            Me.DataGridView1.Rows(indice).Cells.Item("Descripcion").Value = ProdRows(0).Item(1)
            Me.DataGridView1.Rows(indice).Cells.Item("Barra").Value = ProdRows(0).Item(2)
            Me.DataGridView1.Rows(indice).Cells.Item("Precio").Value = ProdRows(0).Item(3)
        Catch ex As IndexOutOfRangeException
            MessageBox.Show("No existe codigo de Producto", ex.Message)
        Catch ex As ArgumentException
            MessageBox.Show("Revise el codigo que ingresa ", ex.Message)
        Finally
            My.Forms.frmPrincipal.objConex.Close()
        End Try
    End Sub

    Private Sub btnProcess_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcess.Click
        Me.btnProcess.Text = "Procesando..."
        Me.DataGridView1.Enabled = False
        'Me.btnProcess.Enabled = False
        Dim Codigo, Barra, NombreCorto, Fecha As String
        Dim Cantidad As Integer
        Dim Pprecio As Double
        'Aca creamos la nueva tabla para luego leer los datos para el reporte
        'la tabla se llamara Etiquetas. 
        Dim pEtiquetas As New DataTable
        'Aca creamos la variable que controlara las filas de nuestra nueva tabla.
        Dim rowEtiquetas As DataRow
        'Aca se le asigna el nombre a la tabla como dijimos será Etiquetas
        pEtiquetas.TableName = "Etiquetas"
        'Aqui agregamos las columnas o encabezados con los que despues podremos llamar
        'los valores almacenados en dichas columnas.
        pEtiquetas.Columns.Add("PedidoID")
        pEtiquetas.Columns.Add("Codigo")
        pEtiquetas.Columns.Add("Cantidad")
        pEtiquetas.Columns.Add("CodigoB")
        pEtiquetas.Columns.Add("Precio")
        pEtiquetas.Columns.Add("CFName")
        pEtiquetas.Columns.Add("Fecha")
        Dim contador, lazo As Integer
        'Dimensiono una variable igual a las filas que tiene el DataGrid.
        contador = Me.DataGridView1.Rows.Count
        If (contador = 1) Then
            MessageBox.Show("Debe agregar un codigo por lo menos")
        Else
            'Aca hago un lazo para llena mi DataSet con la nueva tabla Etiquetas que 
            'es la fuente para generar las viñetas.
            For lazo = 0 To contador - 2
                'esta linea no se porque va pero en todos los ejemplos que revise estaba, asi
                'que no la podia ignorar.Supongo que es para poder tener donde meter valores :P!;
                'si eso es!! hasta que lo escribo pense bien el proque de esta fila!! jejeje.
                'Aqui valido si el grid esta vacio, si es asi no podra proceder a meter valores
                'a la tabla hasta que digite algun codigo.Esto con el fin de no guardar basura.
                'Aqui agrego una nueva fila, se hace tantas veces como filas tenga el DataGrid.
                rowEtiquetas = pEtiquetas.Rows.Add()
                'Aqui cada variable se llena con la info. proveniente del DataGridView.
                Codigo = Me.DataGridView1.Rows(lazo).Cells.Item(0).Value
                Cantidad = Me.DataGridView1.Rows(lazo).Cells.Item(1).Value
                Barra = Me.DataGridView1.Rows(lazo).Cells.Item(2).Value
                If IsDBNull(Me.DataGridView1.Rows(lazo).Cells.Item(4).Value) Then
                    Pprecio = 1.0
                Else
                    Pprecio = Me.DataGridView1.Rows(lazo).Cells.Item(4).Value
                End If
                'Pprecio = Me.DataGridView1.Rows(lazo).Cells.Item(4).Value
                NombreCorto = Me.txtCusName.Text
                Fecha = Me.TxtFecha.Text
                'En estas lineas paso las variables ya llenas con informacion que viene
                'del datagrid hacia la coleccion DataRow llamada rowEtiquetas que llenará
                'la tabla Etiquetas de mi DataSet.
                rowEtiquetas("PedidoID") = Pedido
                rowEtiquetas("Codigo") = UCase(Codigo)
                rowEtiquetas("Cantidad") = Cantidad
                rowEtiquetas("CodigoB") = Barra
                rowEtiquetas("Precio") = Pprecio
                rowEtiquetas("CFName") = NombreCorto
                rowEtiquetas("Fecha") = Fecha
            Next
            'Aqui ya con la tablita conteniendo informacion, la agrego a mi DataSet.
            My.Forms.frmPrincipal.miDataSet.Tables.Add(pEtiquetas)
            'Vamos a abrir la conexion nueva para guardar la info del DataSet en una base 
            'de datos.
            'reservo la nueva variable de conexion
            Dim nConex As SqlConnection
            'raservo la variable de nueva cadena de conexion.
            Dim nStringConex As String
            Dim dAdapter As SqlDataAdapter
            nStringConex = "Data Source=AUXINFORMATICA;Initial Catalog=Etiquetas;Integrated Security=yes"
            nConex = New SqlConnection(nStringConex)
            Try
                nConex.Open()
                dAdapter = New SqlDataAdapter("select * from Etiquetas", nStringConex)
                Dim cmdBuilder As New SqlCommandBuilder(dAdapter)
                dAdapter.Update(My.Forms.frmPrincipal.miDataSet, "Etiquetas")
                MessageBox.Show("Se ha guardado el pedido; su numero es: " & Pedido, "Importante", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "error de conexion local")
            Finally
                nConex.Close()
                Me.Close()
            End Try
            'My.Forms.frmPrincipal.miDataSet.AcceptChanges()
            'Me.Close()
            'Dim frmEtiqueta As New frmLabel
            'frmEtiqueta.MdiParent = frmPrincipal
            'frmEtiqueta.Show()
        End If
    End Sub

    Private Sub txtCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtCancel.Click
        My.Forms.Form1.Dispose()
        Me.Close()
    End Sub
End Class