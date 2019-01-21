﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Gestão_De_requisições
{
    public partial class samuka : Form
    {

        chat mes = new chat();

        int a = 0;//Varaivel contadora para contar o numero de faturas
        bool accept = false;
        bool permicao = false;
        bool permicao2 = false;


        //Nome dos ficheiros necessarios 
        string req_global = @"requniversal.txt";//Ficheiro que se baseia para que o  sistema de alerta de devolucao pendentes  funcione 
        string backup = @"backup.txt";//ajuda na passagem de informacao para o ficheiro req_global
        string conversa = @"conversa.txt";
        string salas = @"salas.txt";
        string objectos = @"objectos.txt";
        string requisicoes =("R_"+DateTime.Today.ToString("yyyy-MM-dd") + ".txt");
        string apoio = @"apoio.txt";


        

        requisitar req = new requisitar();//Permite utilizar esta class em todo o form 
        string linhaglobal;
        string linha3;


        public samuka()
        {
            InitializeComponent();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        //Log out 
        private void button2_Click(object sender, EventArgs e)
        {
            Form f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string linha;//Adiciona ao ficheiro  
            string linha2;//Adiciona a listbox
            chat mes = new chat();
            contas utilizador=new contas();


            mes.utilizador = Program.utilname;
            mes.conversa = textBox9.Text;
            mes.email = Program.utilemail;
            mes.para = "seguranca";


            linha = (mes.utilizador + ";" + mes.email + ";" + mes.conversa + ";" + mes.para);


            //Adiciopna a mensagem de uma forma estruturada no ficheiro 
            StreamWriter sw = File.AppendText(conversa);
            sw.WriteLine(linha);
            sw.Close();


            //adiciona a listbox
            linha2 = ("EU" + ":" + mes.conversa);
            listBox1.Items.Add(linha2);

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_Click(object sender, EventArgs e)
        {
            textBox9.Text = "";
        }

        private void samuka_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            dataGridView1.Rows.Clear();
            string confpassado = "";
            int x = 0;
            int quant=0;
            StreamReader srconf = File.OpenText(req_global);
            while ((confpassado=srconf.ReadLine())!=null)
            {
                string[] fillc = confpassado.Split(';');
                if (true)
                {
                    if (fillc.Length<7 && fillc[0]==Program.utilname)
                    {
                        dataGridView1[0, x].Value = fillc[0];
                        dataGridView1[1, x].Value = fillc[2];
                        dataGridView1[2, x].Value = fillc[1];
                        dataGridView1[3, x].Value = fillc[3];
                        dataGridView1[4, x].Value = fillc[4];
                        dataGridView1[5, x].Value = "-";
                        dataGridView1[6, x].Value = "-";

                        quant++;
                    }
                }
            }
            srconf.Close();
            if (quant>0) 
            {
                MessageBox.Show("Existem objetos por devolver. Caso o objeto por devolver seja um objeto que foi requisitado num outro dia faça a devolução num segurança.","Objetos por Devolver",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                MessageBox.Show("Pode-se ver o objeto ou objetos que estão por devolver, no painel de Objetos por 'devolver'", "Visualizar Objetos por Devolver", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            add_pordev();//Adiciona os elementos que nao foram devolvidos que deveram ser devolvidos 


            StreamReader sr;
            actualizarchat();
            //Retira as salas do ficheiro salas e acrescenta na comboxbox das salas



            sr = File.OpenText(salas);
            string linha;
            while ((linha = sr.ReadLine()) != null)    //OBS: FALTA DE CONSTRUCAO 
            {
                comboBox1.Items.Add(linha);
            }
            sr.Close();


            //Retira os objectos do ficheiro dos objectos e acrescenta na combobox dos objectos
            sr = File.OpenText(objectos);
            string linha2;
            while ((linha2 = sr.ReadLine()) != null) //FALTA DE CONSTRUCAO 
            {

                string[] fill = linha2.Split(';'); 

                comboBox2.Items.Add(fill[1]);
            }
            sr.Close();

            //Acrescenta as mensagens que sao dirigidas ao utilizador em concreto 

        }


        public void add_pordev() {

            string linha;
            StreamReader sr = File.OpenText(requisicoes);
            while ((linha = sr.ReadLine())!=null)
            {

                string[] del = linha.Split(';');//Guarda as informacoes da delivery/ entrega

                if (del[0]==Program.utilname && del.Length<=5)
                {
                    comboBox3.Items.Add(del[3] + "-" + del[4]);
                    permicao = true;
                }

            
            }
            sr.Close();

        }










        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        //preencher o formulário de aquisição 
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Para prosseguir deve-se registrar a sala e o tipo de objeto manualmente", "Formulário incompleto", MessageBoxButtons.OK);
            }
            else { 
            textBox2.Text= Program.utilname;
                textBox3.Text = DateTime.Now.ToString("hh:mm:ss");
                textBox4.Text = DateTime.Today.ToString("yyyy-MM-dd");




                req.docente = textBox2.Text;
                req.data = textBox3.Text;
                req.hora = textBox4.Text;
                req.sala = comboBox1.SelectedItem.ToString();
                req.objeto = comboBox2.SelectedItem.ToString();

                string linha = (req.docente + ";" + req.data + ";" + req.hora + ";" + req.sala + ";" + req.objeto); // variavel que estrutura a informacao para submetela no formulario de forma inicial 
                linhaglobal = linha;//Guarda a linha para que haja uma comparacao na hora da devolucao 

                accept = true;
            }
        }
            


          

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //Preenchimento da entrega para poder ser enviada
        private void button3_Click(object sender, EventArgs e)
        {
            //confirmacao de existencia de registro de aquisicao 
            if (permicao == false)
            {
                MessageBox.Show("Não existem registos de requisições", "Requisitar",MessageBoxButtons.OK);
            }
            else if (comboBox3.SelectedItem==null)
            {
                MessageBox.Show("Seleciona o item que deseja devolver", "Item à devolver", MessageBoxButtons.OK);
            }
            else if(permicao==true)
            {

            preencher();//Invoca o metedo que permite tranferir dados de um file para as variaveis para poder ser devolvido 


            //preenche as partes da devolucao preparando para a entrega 
            textBox1.Text = req.docente;
            textBox8.Text = req.data;
            textBox6.Text = req.hora;
            textBox10.Text = req.sala;
            textBox11.Text = req.objeto;
            //Preenche a hora e data de entrega preparando para ser enviadas para a devolucao essa será adicionada ao ficheiro 
            textBox7.Text = DateTime.Now.ToString("hh:mm:ss");
            textBox5.Text = DateTime.Today.ToString("yyyy-MM-dd");

            req.dataentre = textBox7.Text;
            req.horaentre = textBox5.Text;
            linha3 = (req.docente + ";"+ req.data +";"+ req.hora + ";" + req.sala + ";" + req.objeto + ";" + req.dataentre + ";" + req.horaentre);//a linha que sera submetida no formulario adicionando a hora de entrega
                permicao2 = true;
            }
        }

        public void preencher()
        {
            string A = comboBox3.SelectedItem.ToString();

            string[]  fill2 = A.Split('-');//separa os elementos da combobox3 
            string linha;
            StreamReader sr = File.OpenText(requisicoes);
            while ((linha = sr.ReadLine())!=null)
            {
               string [] fill = linha.Split(';');

                if (fill[0]==Program.utilname && fill[3]==fill2[0] && fill[4]==fill2[1] && fill.Count()<6)
                {
                    req.docente = fill[0];
                    req.data = fill[1];
                    req.hora = fill[2];
                    req.sala = fill[3];
                    req.objeto = fill[4];


                    linhaglobal = (req.docente + ";" + req.data + ";" + req.hora + ";" + req.sala + ";" + req.objeto);
                }
            }
            sr.Close();


        }
        









        //Envia para o ficheiro os dados da devolucao 
        private void button7_Click(object sender, EventArgs e)
        {
            //Cria o ficheiro de apoio impedindo a perda de informacao

            if (permicao2 == false)
            {
                MessageBox.Show("O formulário de devolucão deve ser preenchido neste caso ", "Devolução", MessageBoxButtons.OK);
            }
            else
            {
                StreamWriter sw = File.CreateText(apoio);
                sw.Close();


                string linha;
                StreamReader sr;
                sr = File.OpenText(requisicoes);
                while ((linha = sr.ReadLine()) != null)
                {
                    //ESCREVE NO OURTO FICHEIRO
                    StreamWriter sw2 = File.AppendText(apoio);

                    if (linha == linhaglobal)//caso a linha encontrada for igual a linha escrita no formulario de requisicao 
                    {
                        sw2.WriteLine(linha3);//passa para o outro ficheiro a linha2 sendo que impede com que a linha seja escrita sm data e hora
                        sw2.Close();
                    }
                    else
                    {
                        sw2.WriteLine(linha);//caso nao for encontrada algo igual ele escreve a linha como esta escrita
                        sw2.Close();
                    }

                }
                sr.Close();


                File.Delete(requisicoes);//Elimina o ficheiro anterior para permitir refazelo do zero 
                StreamWriter sw3 = File.CreateText(requisicoes);//Recria o ficheiro para ser tranferido de novo os dados atualizados 
                sw3.Close();

                string linharecr;//escreve de novo no ficheiro 

                sr = File.OpenText(apoio);
                while ((linharecr = sr.ReadLine()) != null)
                {
                    //Devolve os dados para o ficheiro reconstruido 
                    StreamWriter sw2 = File.AppendText(requisicoes);
                    sw2.WriteLine(linharecr);
                    sw2.Close();


                }
                sr.Close();

                File.Delete(apoio);//elimina o ficheiro de apoio 



                //Escreve no ficheiro global de requisicao 
                sr = File.OpenText(req_global);
                while ((linha = sr.ReadLine()) != null)
                {
                    //ESCREVE NO OURTO FICHEIRO
                    StreamWriter sw2 = File.AppendText(backup);

                    if (linha == linhaglobal)//caso a linha encontrada for igual a linha escrita no formulario de requisicao 
                    {
                        sw2.WriteLine(linha3);//passa para o outro ficheiro a linha2 sendo que impede com que a linha seja escrita sm data e hora
                        sw2.Close();
                    }
                    else
                    {
                        sw2.WriteLine(linha);//caso nao for encontrada algo igual ele escreve a linha como esta escrita
                        sw2.Close();
                    }

                }
                sr.Close();


                File.Delete(req_global);//Elimina o ficheiro anterior para permitir refazelo do zero 
                sw3 = File.CreateText(req_global);//Recria o ficheiro para ser tranferido de novo os dados atualizados 
                sw3.Close();

               

                sr = File.OpenText(backup);
                while ((linharecr = sr.ReadLine()) != null)
                {
                    //Devolve os dados para o ficheiro reconstruido 
                    StreamWriter sw2 = File.AppendText(req_global);
                    sw2.WriteLine(linharecr);
                    sw2.Close();


                }
                sr.Close();

                File.Delete(backup);//elimina o ficheiro de apoio


                dataGridView1.Rows.Clear();
                string confpassado = "";
                int x = 0;
                int quant = 0;
                StreamReader srconf = File.OpenText(req_global);
                while ((confpassado = srconf.ReadLine()) != null)
                {
                    string[] fillc = confpassado.Split(';');
                    if (true)
                    {
                        if (fillc.Length < 7 && fillc[0] == Program.utilname)
                        {
                            dataGridView1[0, x].Value = fillc[0];
                            dataGridView1[1, x].Value = fillc[2];
                            dataGridView1[2, x].Value = fillc[1];
                            dataGridView1[3, x].Value = fillc[3];
                            dataGridView1[4, x].Value = fillc[4];
                            dataGridView1[5, x].Value = "-";
                            dataGridView1[6, x].Value = "-";

                            quant++;
                        }
                    }
                }
                srconf.Close();
                if (quant == 0)
                {
                    MessageBox.Show("Todos os objetos foram devolvidos com sucesso.", "Objetos  Devolvidos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    
                }



                //TODAS A TEXTBOX PASSAM A SER NULO DE VOLTA
                textBox1.Text = "";
                textBox8.Text = "";
                textBox6.Text = "";
                textBox10.Text = "";
                textBox11.Text = "";
                textBox7.Text = "";
                textBox5.Text = "";

                comboBox3.Items.Remove(comboBox3.SelectedItem);
                comboBox3.SelectedItem = null;
                permicao2 = false;


                //Conta os itens da combobox caso essa estiver vazia ele impede o utilizador de "preencher" o formulário da entrega
                if (comboBox3.Items.Count == 0)
                {
                    permicao = false;
                }

            }



            




        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (accept==false)
            {
                MessageBox.Show("O formulário não foi totalmentre preenchido", "Formulário Incompleto", MessageBoxButtons.OK);
            }
            else
            {

               

                                            
                if (File.Exists(requisicoes) == true)
                {
                    StreamWriter sw = File.AppendText(requisicoes);
                    sw.WriteLine(linhaglobal);
                    sw.Close();
                }

                 if (File.Exists(req_global) == true)
                  {
                    StreamWriter sw = File.AppendText(req_global);
                    sw.WriteLine(linhaglobal);
                    sw.Close();
                  }


                MessageBox.Show("O formulário foi submetido com sucesso", "Formulário Submetido", MessageBoxButtons.OK);
                permicao = true;
                accept = false;//impede fazer uma nova submicao sem preencher a outra

                //obriga o utilizador a preencher de novo 
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                comboBox1.SelectedItem = null;
                comboBox2.SelectedItem = null;

                // adicionar elementos a combobox3 a combobox da entrega
                string []fill =linhaglobal.Split(';');
                comboBox3.Items.Add(fill[3]+"-"+fill[4]);


                dataGridView1.Rows.Clear();
                string confpassado = "";
                int x = 0;
                int quant = 0;
                StreamReader srconf = File.OpenText(req_global);
                while ((confpassado = srconf.ReadLine()) != null)
                {
                    string[] fillc = confpassado.Split(';');
                    if (true)
                    {
                        if (fillc.Length < 7 && fillc[0] == Program.utilname)
                        {
                            dataGridView1[0, x].Value = fillc[0];
                            dataGridView1[1, x].Value = fillc[2];
                            dataGridView1[2, x].Value = fillc[1];
                            dataGridView1[3, x].Value = fillc[3];
                            dataGridView1[4, x].Value = fillc[4];
                            dataGridView1[5, x].Value = "-";
                            dataGridView1[6, x].Value = "-";

                            quant++;
                        }
                    }
                }
                srconf.Close();

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
          

        }

        //funcao actualizadora de chat
        public void actualizarchat()
        {
            listBox1.Items.Clear();
            chat mes = new chat();



            string linha;
            StreamReader sw = File.OpenText(conversa);

            while ((linha = sw.ReadLine()) != null)
            {
                string[] fill = linha.Split(';');

                if (fill[3] == Program.utilname && fill.Length <=5 && fill[4]==Program.utilemail)//mensagens de segurança ao docente
                {

                    mes.utilizador = fill[0];
                    mes.conversa = fill[2];


                    string linha2 = mes.utilizador + ":" + mes.conversa +":"+"---------------"+"Relacionado a:"+" "+fill[5] ;

                    listBox1.Items.Add(linha2);
                }
                else if(fill.Length<=4 && fill[0]==Program.utilname)
                {
                    listBox1.Items.Add("EU"+":"+fill[2]);
                }
             
                

            }
            sw.Close();
        }



        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button5_Click_1(object sender, EventArgs e)
        {

            if (listBox1.SelectedItem==null)
            {
                MessageBox.Show("E é necessário  selecionar a mensagem que deseja selecionar", "Visualizar", MessageBoxButtons.OK , MessageBoxIcon.Exclamation);
            }
            else if (listBox1.SelectedItem.ToString().Contains("EU"))
            {
                MessageBox.Show("E é necessário  esperar que o problema  levantado, seja resolvido pelos seguranças, para que essa mensagem seja resolvido para que essa menssagem desapareça", "Visualizar Impossível", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {

                DialogResult result = MessageBox.Show("Confirma o seu acto de 'visualizar'? Depois de 'vizualizada' essa mensagem  desaparecerá da sua vistá de utilizador e para visualiza-la de novo deve consultar o serviço de administração "," Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);


                if (result ==DialogResult.Yes)
                {
                    string[] responder = listBox1.SelectedItem.ToString().Split(':');



                    string alterar = "";
                    string comparacao = "";



                    string linha;//linha como esta presente na actualidade  
                    string linha2;

                    //Abre ficheiro para retirar a mensagem e o nome do utilizador para colocar numa nova string que vai ser alterado no ficheiro 
                    StreamReader sr = File.OpenText(conversa);
                    while ((linha = sr.ReadLine()) != null)
                    {
                        string[] fill = linha.Split(';');

                        if (fill[0] == responder[0] && fill[2] == responder[1] && fill.Length <= 6)
                        {
                            alterar = linha + ";" + "V"; // Alteracao para submeter para o ficheiro actualizado  
                            comparacao = linha;

                        }

                    }
                    sr.Close();




                    //Cria o ficheiro de apoio impedindo a perda de informacao ao eliminar  

                    StreamWriter sw = File.CreateText(apoio);
                    sw.Close();




                    sr = File.OpenText(conversa);
                    while ((linha2 = sr.ReadLine()) != null)
                    {
                        //ESCREVE NO OURTO FICHEIRO
                        StreamWriter sw2 = File.AppendText(apoio);

                        if (comparacao == linha2)//caso a linha encontrada for igual a linha escrita no ficheiro
                        {
                            sw2.WriteLine(alterar);//passa para o outro ficheiro a alteracao  sendo que impede com que a linha seja escrita 
                            sw2.Close();
                        }
                        else
                        {
                            sw2.WriteLine(linha2);//caso nao for encontrada algo igual ele escreve a linha como esta escrita
                            sw2.Close();
                        }

                    }
                    sr.Close();


                    File.Delete(conversa);//Elimina o ficheiro anterior para permitir refazelo do zero 
                    StreamWriter sw3 = File.CreateText(conversa);//Recria o ficheiro para ser tranferido de novo os dados atualizados 
                    sw3.Close();

                    string linharecr;//escreve de novo no ficheiro actualizado

                    sr = File.OpenText(apoio);
                    while ((linharecr = sr.ReadLine()) != null)
                    {
                        //Devolve os dados para o ficheiro reconstruido 
                        StreamWriter sw2 = File.AppendText(conversa);
                        sw2.WriteLine(linharecr);
                        sw2.Close();


                    }
                    sr.Close();

                    File.Delete(apoio);//elimina o ficheiro de apoio }



                    MessageBox.Show("O processo foi realizado com sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    actualizarchat();//Actualiza automaticamente a listbox

                }
                else
                {
                    MessageBox.Show("O processo foi cancelado", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }


            }











    }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {



        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Status1.Text = "Data:" + DateTime.Today.ToString("yyyy-MM-dd") + "     " + "Hora:" + DateTime.Now.ToString("hh:mm:ss");
        }
    }
    }
