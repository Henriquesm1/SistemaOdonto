using Entidades;
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
    public partial class frmPrincipal : Form
    {

        ConsultaService service = new ConsultaService();
        DentistaService serviceD = new DentistaService();
        PacienteService serviceP = new PacienteService();

        public frmPrincipal()
        {
            InitializeComponent();
            IniciarFormulario();
        }

        private void IniciarFormulario()
        {

            atualizarCb();
        }


        private void atualizarCb()
        {
            try
            {
                var lista = serviceD.Listar();
                var listaD = new Dictionary<int, string>();
                listaD.Add(0, "Selecione um Dentista");
                foreach (var item in lista)
                {
                    listaD.Add(item.Id, item.Nome);
                }
                cbDentista.DataSource = new BindingSource(listaD, null);
                cbDentista.DisplayMember = "value";
                cbDentista.ValueMember = "key";

                atualizarAgenda(Convert.ToInt32(cbDentista.SelectedValue));

            }
            catch (Exception ex)
            {

                MessageBox.Show("Erro ao carregar a lista!" + ex.Message);
            }
        }

        private void atualizarAgenda(int id)
        {
            Dentista d = new Dentista();
            d = serviceD.Buscar(id);
            if(d != null)
            {
                dg.Rows.Clear();
                gerarConsulta(d);
            }
        }

        private void gerarConsulta(Dentista d)
        {
            var con = service.Buscar(d, Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"))).OrderBy(x => x.HoraMarcada);
            if(con == null)
            {
                MessageBox.Show("Este dentista não tem pacientes hoje!!");

            }
            else
            {
                foreach(var item in con)
                {
                    int linha = dg.Rows.Add();
                    dg.Rows[linha].Cells[0].Value = item.IdConsulta;
                    dg.Rows[linha].Cells[1].Value = item.HoraMarcada.Value.ToString("HH:mm");
                    dg.Rows[linha].Cells[2].Value = serviceP.Buscar(item.IdPaciente).Nome; 
                    dg.Rows[linha].Cells[3].Value = imagemStatus(item.Status);
                    dg.Rows[linha].Cells[4].Value = item.IdPaciente;
                }
            }
        }

        private Bitmap imagemStatus(string s) //Status de consuta
        {
            var imagem = new Bitmap(Properties.Resources.Circle_Grey);
            switch (s)
            {
                case "Confirmado":
                    imagem = new Bitmap(Properties.Resources.Circle_Blue);
                    break;

                case "Desmarcado":
                    imagem = new Bitmap(Properties.Resources.Circle_Red);
                    break;

                case "Nao confirmado":
                    imagem = new Bitmap(Properties.Resources.Circle_Grey);
                    break;

                case "Ja chegou":
                    imagem = new Bitmap(Properties.Resources.Circle_Orange);
                    break;

                case "Em atendimento":
                    imagem = new Bitmap(Properties.Resources.Circle_Green);
                    break;

                default:
                    imagem = new Bitmap(Properties.Resources.address171);
                    break;
            }
            return imagem;
        }

        private void menuDentista_Click(object sender, EventArgs e)
        {
            frmCadDentista frm = new frmCadDentista();
            frm.ShowDialog();
        }

        private void menuAgDentistas_Click(object sender, EventArgs e)
        {
            frmConDentista frm = new frmConDentista();
            frm.ShowDialog();
        }

        private void menuPaciente_Click(object sender, EventArgs e)
        {
            frmCadPaciente frm = new frmCadPaciente();
            frm.ShowDialog();
        }

        private void menuAgPacientes_Click(object sender, EventArgs e)
        {
            frmConPaciente frm = new frmConPaciente();
            frm.ShowDialog();
        }

        private void menuConsulta_Click(object sender, EventArgs e)
        {
            frmCadConsulta frm = new frmCadConsulta();
            frm.ShowDialog();
        }

        private void menuAgConsultas_Click(object sender, EventArgs e)
        {
            frmConsultas frm = new frmConsultas();
            frm.ShowDialog();
        }

        private void menuSuporte_Click(object sender, EventArgs e)
        {
            frmSuporte frm = new frmSuporte();
            frm.ShowDialog();
        }

        private void menuSair_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cbDentista_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cbDentista_SelectionChangeCommitted(object sender, EventArgs e)
        {
            atualizarAgenda(Convert.ToInt32(cbDentista.SelectedValue));
            
        }

        private void dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 2 && e.RowIndex != -1)
            {
                DataGridView dg = sender as DataGridView;
                int id = Convert.ToInt32(dg.Rows[e.RowIndex].Cells[4].Value);
                Paciente p = serviceP.Buscar(id);
                frmEditarPaciente frm = new frmEditarPaciente(p);
                frm.Show();
            }

            if (e.ColumnIndex == 3 && e.RowIndex != -1)//Feedback No GridsView
            {
                int id = Convert.ToInt32(dg.Rows[e.RowIndex].Cells[0].Value);
                Consulta c = service.Buscar(id);
                string s = c.Status;

                switch (s)
                {
                    case "Confirmado":
                        c.Status = "Desmarcado";
                        break;
                    case "Desmarcado":
                        c.Status = "Ja chegou";
                        break;
                    case "Ja chegou":
                        c.Status = "Em atendimento";
                        break;
                    case "Em atendimento":
                        c.Status = "Nao confirmado";
                        break;
                    case "Nao confirmado":
                        c.Status = "Confirmado";
                        break;
                    default:
                        c.Status = "Nao confirmado";
                        break;
                }

                service.Editar(c);
                atualizarAgenda(Convert.ToInt32(cbDentista.SelectedValue));
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAnotacoes frm = new frmAnotacoes();
            frm.ShowDialog();

            RichTextBox post = new RichTextBox();
            post.ReadOnly = true;
            post.BorderStyle = BorderStyle.None;
            post.Font = new Font(FontFamily.GenericSansSerif, 15F);
            post.ForeColor = Color.FromArgb(64, 64, 64);
            post.Width = 200;
            post.Height = 200;
            post.BackColor = frm.cor;
            post.Text = frm.texto;
            anot.Controls.Add(post);
     
        }

        private void cbDentista_Click(object sender, EventArgs e)
        {
            atualizarCb();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
