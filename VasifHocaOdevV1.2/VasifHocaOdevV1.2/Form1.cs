using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Huseyin;
using System.Threading;

namespace VasifHocaOdevV1._2
{
    public partial class Form1 : Form
    {
        ArrayList islemler;
        ArrayList taban;
        Huseyin.Huseyin kutluk = new Huseyin.Huseyin();
        ArrayList kayit = new ArrayList();
        

        public Form1()
        {
            InitializeComponent();
            chdistenen.GetItemChecked(0);
            islemler = new ArrayList { "*", "+", "o", "→","|" };
            panel1.AutoScroll = true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            panel1.Controls.Clear();

            if (chdistenen.CheckedItems.Count == 0)
            {
                MessageBox.Show("taban seçiniz.");
                return;
            }
            if (!YazınınKontrolu(textBox1.Text))
            {
                MessageBox.Show("Yazdığınız Metinle İlgili Bir Sıkıntı Var");
                return;
            }
            
            ilk_dugum = new agacim();
            gecerlidugum = ilk_dugum;

            richTextBox1.Text = "Herşey İstenen Taba Dönüşüyor\n\n\n\n\n";

            //MessageBox.Show(kutluk.Sadelestir(textBox1.Text));
            
            textBox1.Text = degilin_degili(textBox1.Text);
            string isleme_girecek = textBox1.Text;
            kayit.Add(isleme_girecek);
            string donusturulmus = Ayikla(isleme_girecek, 0);
            kayit.Add(donusturulmus);

            #region Sadeleştirme

            string sonuc = donusturulmus;
            
            sonuc = degilin_degili(sonuc);

            if (chckdcontrol("o"))
            {
                sonuc = NorSadelestir(donusturulmus);
                kayit.Add(sonuc);
            }
            else if (chckdcontrol("|"))
            {
                sonuc = NandSadelestir(donusturulmus);
            }
            else if (chckdcontrol("+"))
            {
                // SADELEŞTİRME İŞLEMLERİ BURADA YAPILMALI
            }
            else if (chckdcontrol("*"))
            {
                // SADELEŞTİRME İŞLEMLERİ BURADA YAPILMALI
            }

            #endregion
            
            sonuc = degilin_degili(sonuc);

            sonuc = Gereksiz_Parantezlerin_Atilmasi(sonuc);

            richTextBox1.Text += "\n\nsonuc:\n" + sonuc;
            kayit.Add(sonuc);


            
            
            #region Çizdirme Fonksiyonlarının Çağırılması

            panel1.Controls.Clear();
            Ayikla(sonuc, 1);
            olustur2(ilk_dugum, 0, 0, 0);
            olustur2(ilk_dugum, 0, 0, 1);


            #endregion
        
        }

        

        #region Kontroller

        Boolean YazınınKontrolu(string kontroledilecek)
        {
            int parantez_sayisi = 0;
            
            for (int i = 0; i < kontroledilecek.Length; i++)
            {
                if (i != kontroledilecek.Length-1 &&control(kontroledilecek[i].ToString()) && control(kontroledilecek[i + 1].ToString()))
                {
                    return false;
                }
                if (kontroledilecek[i] == '(')
                {
                    parantez_sayisi++;
                }
                else if (kontroledilecek[i] == ')')
                {
                    parantez_sayisi--;
                }
            }
            if (parantez_sayisi != 0) return false;
            if (control(kontroledilecek[0].ToString())) return false;
            return true;
        }

        Boolean control(string metin)
        {
            return islemler.Contains(metin);
        }

        Boolean chckdcontrol(string metin)
        {
            return chdistenen.CheckedItems.Contains(metin);
        }

        Boolean metin_ne(string metin)
        {
            for (int i = 0; i < metin.Length; i++)
            {
                if (control(metin[i].ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
       
        #region AYIKLA BİRLEŞTİR DEĞİŞTİR

        string Ayikla(string metin, int asama)
        {
            ArrayList dizi = new ArrayList();

            for (int i = 0; i < metin.Length; i++)
            {
                if (control(metin[i].ToString()))
                {
                    int silinecek = 0;
                    dizi.Add(metin.Substring(0, i));
                    if (i != metin.Length - 1)
                    {
                        dizi.Add(metin.Substring(i, 1));
                        silinecek = 1;
                    }
                    metin = metin.Remove(0, i + 1);
                    i = -1;
                }
                else if (metin[i] == '(')
                {
                    int[] parantezler;
                    parantezler = parantezin_karsılıgı(metin, i);
                    if (metin.Length - 1 >= parantezler[1] + 1 && metin[parantezler[1] + 1] == '\'')
                    {
                        int silinecek = 2;
                        dizi.Add(metin.Substring(0, parantezler[1] + 2));
                        if (metin.Length - 1 != parantezler[1] + 1)  //PARANTEZEDN SONRA + * FELAN İŞARETLERİN GELMESİ DURUMU
                        {
                            dizi.Add(metin.Substring(parantezler[1] + 2, 1));
                            silinecek = 3;
                        }
                        metin = metin.Remove(0, parantezler[1] + silinecek);

                    }
                    else
                    {
                        int silinecek = 1;
                        dizi.Add(metin.Substring(0, parantezler[1] + 1));
                        if (metin.Length - 1 != parantezler[1])  //PARANTEZEDN SONRA + * FELAN İŞARETLERİN GELMESİ DURUMU
                        {
                            dizi.Add(metin.Substring(parantezler[1] + 1, 1));
                            silinecek = 2;
                        }

                        metin = metin.Remove(0, parantezler[1] + silinecek);
                    }
                    i = -1;
                }
            }
            if (metin != "") dizi.Add(metin);
            if (asama == 0)
            {
                return Birlestir(dizi);
            }
            else if (asama == 1)
            {
                cizdirme(dizi);
            }
            return "";
        }

        string Birlestir(ArrayList dizi)
        {

            if (!chckdcontrol("\'"))
            {
                for (int i = 0; i < dizi.Count; i++)
                {
                    if (dizi[i].ToString()[dizi[i].ToString().Length - 1] == '\'')
                        Degistir(dizi[i].ToString(), null, i, dizi);
                }
            }


            for (int i = 0; i < dizi.Count; i++)
            {
                if (dizi[i].ToString() != "")
                    if (dizi[i].ToString()[0] == '(')
                    {
                        string parantezli = dizi[i].ToString();
                        parantezli = parantezli.Remove(parantezli.Length - 1, 1);
                        parantezli = parantezli.Remove(0, 1);
                        dizi[i] = "(" + Ayikla(parantezli, 0) + ")";
                    }
            }

            #region OPTİMİZE EDİLECEK

            for (int i = 0; i < dizi.Count; i++)
            {
                if ((dizi[i].ToString() == "*" && !chckdcontrol(dizi[i].ToString())) || (dizi[i].ToString() == "o" && !chckdcontrol(dizi[i].ToString())))
                {
                    int eskisayi = dizi.Count;
                    string eklenecek = Degistir(dizi[i - 1].ToString(), dizi[i + 1].ToString(), i, dizi);
                    if (dizi.Count != eskisayi) i = 0;
                }
            }

            for (int i = 0; i < dizi.Count; i++)
            {
                if (control(dizi[i].ToString()) && !chckdcontrol(dizi[i].ToString()))
                {
                    int eskisayi = dizi.Count;
                    string eklenecek = Degistir(dizi[i - 1].ToString(), dizi[i + 1].ToString(), i, dizi);
                    if (dizi.Count != eskisayi) i = 0;
                }
            }

            #endregion
            
            string donecek = "";

            for (int i = 0; i < dizi.Count; i++)
            {
                donecek += dizi[i].ToString();
            }

            return donecek;
        }

        string Degistir(string a, string b, int gelen, ArrayList dizi)
        {
            string kullanılacak = "";    richTextBox1.Text += "// --     " + a + dizi[gelen].ToString() + b + "\n";
            

            if (dizi[gelen].ToString() == "+" && !chckdcontrol("+"))
            {
                if (chckdcontrol("o")) kullanılacak = "(" + a + "o" + b + ")o(" + a + "o" + b + ")";
                else if (chckdcontrol("|")) kullanılacak = "(" + a + "|1)|(" + b + "|1)";
                else if (chckdcontrol("*")) kullanılacak = "(" + a + "'*" + b + "')";
                else if (chckdcontrol("→")) kullanılacak = "(" + a + "→" + "0)→"+ b +"";
                dizi.RemoveAt(gelen + 1);
                dizi.RemoveAt(gelen);
                dizi.RemoveAt(gelen - 1);
                dizi.Insert(gelen - 1, kullanılacak);
            }
            else if (dizi[gelen].ToString() == "*" && !chckdcontrol("*"))
            {
                if (chckdcontrol("o")) kullanılacak = "(" + a + "o" + a + ")o(" + b + "o" + b + ")";
                else if (chckdcontrol("|")) kullanılacak = "(" + a + "|"+b+")|1";
                else if (chckdcontrol("+")) kullanılacak = "(" + a + "'+" + b + "')";
                else if (chckdcontrol("→")) kullanılacak = "(" + a + "→" + "("+ b + "→0)→0)" ;
                dizi.RemoveAt(gelen + 1);
                dizi.RemoveAt(gelen);
                dizi.RemoveAt(gelen - 1);
                dizi.Insert(gelen - 1, kullanılacak);
            }
            else if (dizi[gelen].ToString() == "→" && !chckdcontrol("→"))
            {
                if (chckdcontrol("+") && chckdcontrol("\'")) kullanılacak = "(" + a + "'+" + b + ")";
                else if (chckdcontrol("*") && chckdcontrol("\'")) kullanılacak = "(" + a + "*" + b + "')'";
                else if (chckdcontrol("o")) kullanılacak = "((" + a + "o" + a + ")o" + b + ")o((" + a + "o" + a + ")o" + b + ")";
                dizi.RemoveAt(gelen + 1);
                dizi.RemoveAt(gelen);
                dizi.RemoveAt(gelen - 1);
                dizi.Insert(gelen - 1, kullanılacak);
            }
            else if (dizi[gelen].ToString()[dizi[gelen].ToString().Length - 1] == '\'' && !chckdcontrol("\'"))
            {
                if (chckdcontrol("o"))
                {
                    kullanılacak = "(" + a.Remove(a.Length - 1, 1) + "o" + a.Remove(a.Length - 1, 1) + ")";

                    dizi.RemoveAt(gelen);
                    dizi.Insert(gelen, kullanılacak);
                }
                if (chckdcontrol("|"))
                {
                    kullanılacak = "(" + a.Remove(a.Length - 1, 1) + "|1)";

                    dizi.RemoveAt(gelen);
                    dizi.Insert(gelen, kullanılacak);
                }
            }


            #region Eklenecekler
            //else if (dizi[gelen].ToString() == "o" && !chckdcontrol("o"))
            //{
            //if (chckdcontrol("*")) kullanılacak = "(" + a + "o" + a + ")o(" + b + "o" + b + ")";
            //else if (chckdcontrol("+")) kullanılacak = "(" + a + "'+" + b + "')";
            //else if (chckdcontrol("→")) kullanılacak = "((" + a + "→" + "(" + b + "→0)→0)";
            //    dizi.RemoveAt(gelen + 1);
            //    dizi.RemoveAt(gelen);
            //    dizi.RemoveAt(gelen - 1);
            //    dizi.Insert(gelen - 1, kullanılacak);
            //}

            //else if (dizi[gelen].ToString() == "←" && !chckdcontrol("←"))
            //{
            //    string kullanılacak = "(" + a + "o" + a + ")o(" + b + "o" + b + ")";
            //    dizi.RemoveAt(gelen + 1);
            //    dizi.RemoveAt(gelen);
            //    dizi.RemoveAt(gelen - 1);
            //    dizi.Insert(gelen - 1, kullanılacak);
            //}

            #endregion



            string donecek = "";
            if (dizi.Count == 1)
                donecek = dizi[0].ToString();
            else
            {
                for (int i = 0; i < dizi.Count; i++)
                {
                    donecek += dizi[i];
                }
            }
            richTextBox1.Text += "**  " + donecek + "\n\n\n\n";
            return donecek;

        }

        #endregion
        
        #region PARANTEZLERLE İLGİLİ OLAYLAR

        int[] parantezin_karsılıgı(string gelen, int index)
        {
            ArrayList parantezler = new ArrayList();
            parantezler.Add(index);
            int[] donus = new int[2];

            try
            {
                for (int i = index + 1; i < gelen.Length; i++)
                {
                    if (gelen[i] == '(')
                        parantezler.Add(i);
                    else if (gelen[i] == ')')
                    {
                        if (parantezler.Count != 1)
                            parantezler.RemoveAt(parantezler.Count - 1);
                        else
                        {
                            parantezler.Add(i);
                            break;
                        }

                    }

                }


                donus[0] = Convert.ToInt32(parantezler[0]);
                donus[1] = Convert.ToInt32(parantezler[1]);
                return donus;
            }
            catch (Exception)
            {
                MessageBox.Show("Parantezlerle İlgili Bir Sıkıntı Var!!");
                return new int[] {-1};
            }
            return donus;
        }

        string Gereksiz_Parantezlerin_Atilmasi(string metin)
        {
            for (int i = 0; i < metin.Length; i++)
            {
                if (metin[i] == '(' && metin[i + 1] == '(')
                {
                    if (metin[parantezin_karsılıgı(metin, i)[1]] == ')' && metin[parantezin_karsılıgı(metin, i + 1)[1]] == ')')
                    {
                        metin = metin.Remove(i, 1);
                        metin = metin.Remove(parantezin_karsılıgı(metin, i)[1], 1);
                    }
                }
            }
            return metin;
        }

        #endregion
        
        #region Sadelestirme

        string degilin_degili(string metin)
        {
            for (int i = 0; i < metin.Length; i++)
            {
                if (i != metin.Length - 1 && metin[i] == '\'' && metin[i + 1] == '\'')
                {
                    metin = metin.Remove(i, 2);
                }
            }
            return metin;
        }

        #region Nor Sadeleştirme

        string NorSadelestir(string metin)
        {
            //richTextBox1.Text += "\n SADELEŞTİRME İŞLEMİ BAŞLIYOR\n\n\n";
            int[] parantezler;
            int metinin_uzunlugu = metin.Length;
            for (int i = 0; i < metinin_uzunlugu; i++)
            {
                if (metin[i] == '(')
                {
                    parantezler = parantezin_karsılıgı(metin, i);

                    string eski = metin;
                    string giden = metin.Substring(parantezler[0] + 1, parantezler[1] - parantezler[0] - 1);
                    string donen = sadelestir(giden);

                    metin = metin.Substring(0, parantezler[0] + 1) + donen + metin.Substring(parantezler[1] + 1 - 1, metin.Length - parantezler[1]);
                    metinin_uzunlugu = metin.Length;
                    if (eski != metin)
                    {
                        richTextBox1.Text += "//---- " + giden + "\n**  " + donen + " \n\n\n";
                        i = 0;
                    }

                }
            }
            return metin;
        }

        string sadelestir(string metin)
        {

            if (metin == "") return "";
            string a = metin.Substring(0, (metin.Length / 2));
            string b = metin.Substring((metin.Length / 2) + 1, metin.Length - (metin.Length / 2) - 1);
            if (a == b && a.Length != 1)
            {
                string c = "";
                string d = "";
                if (a != "" && a[0] == '(')
                {
                    if (parantezin_karsılıgı(a, 0)[1] == a.Length - 1)
                    {
                        c = a.Substring(1, (a.Length / 2) - 1);
                        d = a.Substring((a.Length / 2) + 1, a.Length - (a.Length / 2) - 2);
                    }
                    else
                    {
                        c = a.Substring(0, (a.Length / 2));
                        d = a.Substring((a.Length / 2) + 1, a.Length - (a.Length / 2) - 1);
                    }
                }
                else
                {
                    c = a.Substring(0, (a.Length / 2));
                    d = a.Substring((a.Length / 2) + 1, a.Length - (a.Length / 2) - 1);
                }
                if (c == d)
                {
                    return c;
                }
            }
            return metin;
        }

        #endregion


        #region Nand Sadelestirme

        string NandSadelestir(string metin)
        {
            int[] parantezler;
            int metinin_uzunlugu = metin.Length;
            for (int i = 0; i < metinin_uzunlugu; i++)
            {
                if (metin[i] == '(')
                {
                    parantezler = parantezin_karsılıgı(metin, i);

                    string eski = metin;
                    string giden = metin.Substring(parantezler[0] + 1, parantezler[1] - parantezler[0] - 1);
                    string donen = sadelestirNand(giden);

                    metin = metin.Substring(0, parantezler[0] + 1) + donen + metin.Substring(parantezler[1] + 1 - 1, metin.Length - parantezler[1]);
                    metinin_uzunlugu = metin.Length;
                    if (eski != metin)
                    {
                        richTextBox1.Text += "//---- " + giden + "\n**  " + donen + " \n\n\n";
                        i = 0;
                    }

                }
            }
            return metin;
        }

        string sadelestirNand(string metin)
        {

            if (metin == "") return "";
            string a = metin.Substring(0, (metin.Length / 2));
            string b = metin.Substring((metin.Length / 2) + 1, metin.Length - (metin.Length / 2) - 1);
            if (a == b)
            {
                return a.Substring(1,a.Length-4);
            }
            return metin;
        }

        #endregion

        #endregion

        #region çizdirme

        agacim ilk_dugum = new agacim();
        agacim gecerlidugum;
        Graphics gr;
        Pen firca;

        void cizdirme(ArrayList dizi)
        {
            agacim atanacaksol = new agacim();
            agacim atanacaksag = new agacim();
            atanacaksag.ata = gecerlidugum;
            atanacaksol.ata = gecerlidugum;


            if (dizi.Count == 1)
            {
                if (dizi[0].ToString() != "")
                {
                    if (dizi[0].ToString()[0] == '(')
                    {
                        dizi[0] = dizi[0].ToString().Remove(0, 1);
                        dizi[0] = dizi[0].ToString().Remove(dizi[0].ToString().Length - 1, 1);

                    }
                    if (metin_ne(dizi[0].ToString()))
                    {
                        Ayikla(dizi[0].ToString(), 1);
                    }
                    else
                    {
                        gecerlidugum.value = dizi[0].ToString();
                    }
                }
                return;
            }
            else if (dizi.Count == 3)
            {
                ArrayList yenidizi = new ArrayList();
                if (dizi[0].ToString() == dizi[2].ToString())
                {
                    gecerlidugum.value = dizi[1].ToString() + "=";

                    gecerlidugum.sagcocuk = atanacaksag;
                    gecerlidugum = atanacaksag;
                    Ayikla(dizi[0].ToString(), 1);
                }
                else
                {
                    gecerlidugum.value = dizi[1].ToString();

                    gecerlidugum.sagcocuk = atanacaksag;
                    agacim dondugundekullan = gecerlidugum;
                    gecerlidugum = atanacaksag;

                    Ayikla(dizi[0].ToString(), 1);

                    gecerlidugum = dondugundekullan;
                    gecerlidugum.solcocuk = atanacaksol;
                    gecerlidugum = atanacaksol;
                    Ayikla(dizi[2].ToString(), 1);
                }

            }
            else if (dizi.Count > 3)
            {
                ArrayList yeniarray = new ArrayList();

                string a = "(";

                int kesilecek = (dizi.Count + 1) / 2;
                if (kesilecek % 2 != 0)
                {
                    kesilecek = (dizi.Count - 1) / 2;
                }
                for (int j = 0; j < kesilecek - 1; j++)
                {
                    a += dizi[j].ToString();
                }
                yeniarray.Add(a + ")");
                yeniarray.Add(dizi[kesilecek - 1].ToString());

                string b = "(";
                for (int j = kesilecek; j < dizi.Count; j++)
                {
                    b += dizi[j].ToString();
                }
                b += ")";
                yeniarray.Add(b);
                cizdirme(yeniarray);
            }
        }
        
        int olustur2(agacim dugum, int y, int x,int hangisi)
        {
             gr = panel1.CreateGraphics();
             firca = new Pen(Color.Blue, 3);
            Point point1;
            Point point2;
            
            int konumy = y;
            int konumx = x;

            int konumatax = konumx;
            int konumatay = konumy;

            if(hangisi==0)
            butonolustur(konumx, konumy, dugum.value);
            konumy += 60;

            if (dugum.solcocuk != null)
            {
                if (hangisi==1)
                {

                    point1 = new Point(konumatax + 25, konumatay + 25);
                    point2 = new Point(konumx + 25, konumy + 25);
                    gr.DrawLine(firca, point1, point2);
                    firca.Dispose();
                    gr.Dispose();
                }
                
                konumx = olustur2(dugum.solcocuk, konumy, konumx,hangisi);

            }
            if (dugum.sagcocuk != null)
            {
                if (hangisi==1)
                {
                    gr = panel1.CreateGraphics();
                    firca = new Pen(Color.Blue, 3);
                    point1 = new Point(konumatax + 25, konumatay + 25);
                    point2 = new Point(konumx + 25, konumy + 25);
                    gr.DrawLine(firca, point1, point2);
                    firca.Dispose();
                }
                
                konumx = olustur2(dugum.sagcocuk, konumy, konumx, hangisi);
            }

            return konumx + 60;
        }
        
        void butonolustur(int konumx, int konumy, string text)
        {
            Button button = new Button();
            button.Location = new System.Drawing.Point(konumx, konumy);
            button.Name = "button1";
            button.ForeColor = Color.Red;
            //button.Enabled = false;
            button.Size = new System.Drawing.Size(50, 50);
            button.Text = text;
            button.UseVisualStyleBackColor = true;
            panel1.Controls.Add(button);
        }

        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            olustur2(ilk_dugum, panel1.AutoScrollPosition.Y, panel1.AutoScrollPosition.X, 1);
        }

        #endregion

        private void btnTexteYazi_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b.Text.ToLower() == "sil")
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1, 1);
            }
            else
                textBox1.Text += b.Text;
        }
        
    }

    public class agacim
    {
        public string value = "";
        public agacim ata;
        public agacim sagcocuk;
        public agacim solcocuk;
    }
    
}