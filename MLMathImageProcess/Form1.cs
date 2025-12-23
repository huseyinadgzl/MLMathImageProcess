using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoruntuIslemeProjesi
{
    public partial class Form1 : Form
    {
        private PictureBox pbKaynak;
        private PictureBox pbHedef;
        private ComboBox cmbFiltreler;
        private Button btnYukle;
        private Button btnIsle;
        private Label lblOk;

        public Form1()
        {
            this.Text = "ML Math: Image Convolution (Görüntü İşleme)";
            this.Size = new Size(850, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 40, 40); 

            ArayuzuOlustur();
        }

        private void ArayuzuOlustur()
        {
            pbKaynak = new PictureBox();
            pbKaynak.Size = new Size(300, 300);
            pbKaynak.Location = new Point(20, 50);
            pbKaynak.SizeMode = PictureBoxSizeMode.StretchImage;
            pbKaynak.BorderStyle = BorderStyle.FixedSingle;
            pbKaynak.BackColor = Color.Gray;
            this.Controls.Add(pbKaynak);

            lblOk = new Label();
            lblOk.Text = "=>";
            lblOk.Font = new Font("Consolas", 20, FontStyle.Bold);
            lblOk.ForeColor = Color.White;
            lblOk.AutoSize = true;
            lblOk.Location = new Point(370, 180);
            this.Controls.Add(lblOk);

            pbHedef = new PictureBox();
            pbHedef.Size = new Size(300, 300);
            pbHedef.Location = new Point(480, 50);
            pbHedef.SizeMode = PictureBoxSizeMode.StretchImage;
            pbHedef.BorderStyle = BorderStyle.FixedSingle;
            pbHedef.BackColor = Color.Gray;
            this.Controls.Add(pbHedef);

            btnYukle = new Button();
            btnYukle.Text = "Resim Seç";
            btnYukle.Location = new Point(20, 360);
            btnYukle.Size = new Size(300, 40);
            btnYukle.BackColor = Color.Orange;
            btnYukle.FlatStyle = FlatStyle.Flat;
            btnYukle.Click += BtnYukle_Click;
            this.Controls.Add(btnYukle);

            cmbFiltreler = new ComboBox();
            cmbFiltreler.Items.AddRange(new object[] { "Kenar Bulma (Edge Detection)", "Keskinleştirme (Sharpen)", "Bulanıklaştırma (Blur)", "Kabartma (Emboss)" });
            cmbFiltreler.SelectedIndex = 0;
            cmbFiltreler.Location = new Point(480, 360);
            cmbFiltreler.Size = new Size(180, 30);
            cmbFiltreler.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbFiltreler);

            btnIsle = new Button();
            btnIsle.Text = "MATRİSİ UYGULA";
            btnIsle.Location = new Point(670, 360);
            btnIsle.Size = new Size(110, 30);
            btnIsle.BackColor = Color.LightGreen;
            btnIsle.FlatStyle = FlatStyle.Flat;
            btnIsle.Click += BtnIsle_Click;
            this.Controls.Add(btnIsle);
        }

        private void BtnYukle_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pbKaynak.Image = new Bitmap(ofd.FileName);
            }
        }

        private void BtnIsle_Click(object sender, EventArgs e)
        {
            if (pbKaynak.Image == null)
            {
                MessageBox.Show("Önce bir resim yükleyin!");
                return;
            }

            btnIsle.Text = "Hesaplanıyor...";
            btnIsle.Enabled = false;
            Application.DoEvents();

            Bitmap kaynakResim = (Bitmap)pbKaynak.Image;

            double[,] kernel = null;

            switch (cmbFiltreler.SelectedIndex)
            {
                case 0: 
                    kernel = new double[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                    break;
                case 1: 
                    kernel = new double[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
                    break;
                case 2: 
                    kernel = new double[,] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
                    break;
                    kernel = new double[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                    break;
            }

            Bitmap sonucResim = KonvolusyonUygula(kaynakResim, kernel);
            pbHedef.Image = sonucResim;

            btnIsle.Text = "MATRİSİ UYGULA";
            btnIsle.Enabled = true;
        }

        
        private Bitmap KonvolusyonUygula(Bitmap resim, double[,] matris)
        {
            if (resim.Width > 800) resim = new Bitmap(resim, new Size(800, (int)(resim.Height * ((double)800 / resim.Width))));

            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);
            int genislik = resim.Width;
            int yukseklik = resim.Height;

            double toplamAgirlik = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    toplamAgirlik += matris[i, j];

            if (toplamAgirlik <= 0) toplamAgirlik = 1; 

            for (int x = 1; x < genislik - 1; x++)
            {
                for (int y = 1; y < yukseklik - 1; y++)
                {
                    double rToplam = 0, gToplam = 0, bToplam = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color piksel = resim.GetPixel(x + i, y + j);
                            double deger = matris[i + 1, j + 1];

                            rToplam += piksel.R * deger;
                            gToplam += piksel.G * deger;
                            bToplam += piksel.B * deger;
                        }
                    }

                    int r = (int)(rToplam / toplamAgirlik);
                    int g = (int)(gToplam / toplamAgirlik);
                    int b = (int)(bToplam / toplamAgirlik);

                    r = Math.Min(255, Math.Max(0, r));
                    g = Math.Min(255, Math.Max(0, g));
                    b = Math.Min(255, Math.Max(0, b));

                    yeniResim.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return yeniResim;
        }
    }
}