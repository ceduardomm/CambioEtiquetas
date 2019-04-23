Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Public Class Form1
    Private Sub btnCerrar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCerrar.Click
        Me.Dispose()
        My.Forms.frmPrincipal.Dispose()
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MdiParent = frmPrincipal
        'funcion que averigua las instancias SQL a las que se tiene acceso en la red
        'crea un arreglo y con cada item de dicho arreglo llena un comboCox
        'Actualizacion a FEB/2019, por alguna razón no encuentro ninguna instancia en la red por lo que 
        'deshabilité esta funcion en la aplicación.
        'Dim factory As Common.DbProviderFactory = Common.DbProviderFactories.GetFactory("System.Data.SqlClient")
        'Dim dataSourceEnumerator As Sql.SqlDataSourceEnumerator = factory.CreateDataSourceEnumerator()
        'Dim dataSourceEnumerator As SqlDataSourceEnumerator = SqlDataSourceEnumerator.Instance
        'Dim dataSources As DataTable = dataSourceEnumerator.GetDataSources()
        'For Each row As DataRow In dataSources.Rows
        'cmbServer.Items.Add(row(0))
        '    Next
        cmbServer.Items.Add("SWCSERVER")
    End Sub
    Private Sub btnConectar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConectar.Click
        If (Me.txtUser.Text = "") Or (Me.txtPsswd.Text = "") Or (Me.txtDB.Text = "") Or (Me.cmbServer.SelectedItem = "") Then
            MessageBox.Show("Necesario ingresar usuario,clave, servidor y Base de datos", "Ojo", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            'My.Forms.frmPrincipal.StringCon = "Data Source='" & cmbServer.SelectedItem & "';Initial Catalog='" & txtDB.Text & "'; User Id='" & txtUser.Text & "';Password='" & txtPsswd.Text & "';"
            My.Forms.frmPrincipal.StringCon = "Server=SWCServer ;Database='" & txtDB.Text & "'; User Id='" & txtUser.Text & "';Password='" & txtPsswd.Text & "';"
            My.Forms.frmPrincipal.objConex = New SqlConnection(My.Forms.frmPrincipal.StringCon)
            Try
                My.Forms.frmPrincipal.objConex.Open()
                Dim frmStart As New frmInicial
                frmStart.Show()
                Me.Dispose()
            Catch ex As ArgumentException
                MessageBox.Show("A ocurrido un error", ex.Message.ToString)
            Catch ex As InvalidExpressionException
                MessageBox.Show("Expresion invalida", ex.InnerException.ToString)
            Catch ex As SqlException
                MessageBox.Show("Usuario o clave invalida", ex.Message)
            Catch ex As Exception
                MessageBox.Show("Hubo error con el servidor por favor revise", ex.Message)
            Finally
                My.Forms.frmPrincipal.objConex.Close()
            End Try
        End If
    End Sub
End Class
