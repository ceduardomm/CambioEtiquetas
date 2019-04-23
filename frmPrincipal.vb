Imports System.Data
Imports System.Data.SqlClient
Public Class frmPrincipal
    Public miDataSet As DataSet
    Public StringCon As String
    Public objConex As SqlConnection
    Private Sub frmPrincipal_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim frmLogin As New Form1
        frmLogin.Show()
    End Sub
    Private Sub SalirToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SalirToolStripMenuItem.Click
        Me.Dispose()
    End Sub
    Private Sub CapturaDeDatosToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CapturaDeDatosToolStripMenuItem.Click
        My.Forms.frmInicial.Show()
    End Sub

    Private Sub InfoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InfoToolStripMenuItem.Click
        Dim frmQuienSoy As New AboutMe
        frmQuienSoy.Show()
    End Sub
End Class