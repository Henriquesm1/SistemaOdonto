﻿using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfService;

namespace SistemaOdonto
{
    public partial class frmEditarPaciente : Form
    {

        public string status;
        Paciente obj = new Paciente();
        PacienteService service = new PacienteService();

        public frmEditarPaciente(Paciente obj)
        {
            InitializeComponent();
            IniciarFormulario(obj);
        }

        private void IniciarFormulario(Paciente objP)
        {
            this.obj = objP;
            lblCodigo.Text = this.obj.Id.ToString();
            txtNome.Text = this.obj.Nome;
            txtEmail.Text = this.obj.Email;
            txtTelefone.Text = this.obj.Telefone.ToString();
            txtCelular.Text = this.obj.Celular.ToString();
            txtCEP.Text = this.obj.CEP;
            txtEndereco.Text = this.obj.Endereco;
            txtComplemento.Text = this.obj.Complemento;
            cbSexo.Text = this.obj.Sexo;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            tsNenhuma.Text = "";
            ts.Text = ValidarCad();
            if (ts.Text == "Sucesso")
            {
                if (lblCodigo.Text != this.obj.Id.ToString())
                {
                    status = "apagado";
                    MessageBox.Show("Este Registro acabou de ser excluido por outro usuário");
                }
                else
                {
                    status = "editado";
                    this.obj.Nome = txtNome.Text;
                    this.obj.Email = txtEmail.Text;
                    obj.Telefone = txtTelefone.Text != "" ? Convert.ToInt64(txtTelefone.Text) : 0;
                    obj.Celular = txtCelular.Text != "" ? Convert.ToInt64(txtCelular.Text) : 0;
                    this.obj.CEP = txtCEP.Text;
                    this.obj.Endereco = txtEndereco.Text;
                    this.obj.Complemento = txtComplemento.Text;
                    this.obj.Nascimento = Convert.ToDateTime(dtDataNasc.Text);
                    this.obj.Sexo = cbSexo.Text;


                    service.Editar(this.obj);
                    this.Close();
                }
            }

        }


        private string ValidarCad()
        {
            ts.ForeColor = Color.Red;
            if (txtNome.Text == string.Empty)
            {
                return "Preencha o campo Nome!";
            }
            else if (txtCelular.Text == string.Empty)
            {
                return "Preencha o campo Celular";
            }
            else if (txtTelefone.Text == string.Empty)
            {
                return "Preencha o campo Telefone";
            }
            else if (txtEmail.Text == string.Empty)
            {
                return "Preencha o campo Email";
            }
            
            else
            {
                ts.ForeColor = Color.Black;
                return "Sucesso";
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            tsNenhuma.Text = "";
            if (ValidarExclusao())
            {
                service.Deletar(this.obj.Id);
                MessageBox.Show("Excluido com sucesso!");
                status = "apagado";
                this.Close();
            }
        }


        public bool ValidarExclusao()
        {
            DialogResult con = MessageBox.Show("Deseja excluir este registro?", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            if (con.ToString().ToUpper() == "YES")
                return true;
            else
                return false;
        }
    }
}
