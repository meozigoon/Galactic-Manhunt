﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_test
{
    public partial class ItemSynthesis : Form
    {
        TaskSelection form;
        private double hydrogen1 = 0;
        private double hydrogen2 = 0;
        private double nitrogen = 0;
        private double oxygen = 0;
        private double epsilonCrystal = 0;
        public ItemSynthesis(TaskSelection form)
        {
            InitializeComponent();
            this.form = form;

            // 보유 자원 목록
            dataGridView1.Rows.Add("수소", 0);
            dataGridView1.Rows.Add("질소", 0);
            dataGridView1.Rows.Add("산소", 0);
            dataGridView1.Rows.Add("엑실론-크리스탈", 0);

            // 아이템 목록, 질량비
            dataGridView3.Rows.Add("퍼옥사이드", "수소 : 산소 = 1 : 8");
            dataGridView3.Rows.Add("하이드라진", "수소 : 질소 = 1 : 7");
            dataGridView3.Rows.Add("엑실론", "엑실론-크리스탈");

            // 필요 자원 목록
            dataGridView2.Rows.Add("수소", 0);
            dataGridView2.Rows.Add("질소", 0);
            dataGridView2.Rows.Add("산소", 0);
            dataGridView2.Rows.Add("엑실론-크리스탈", 0);
        }

        private void button1_Click(object sender, EventArgs e) // 합성
        {

        }

        private void ItemSynthesis_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // 엔터 누르면 합성
            {
                button1_Click(sender, e); // 합성
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) // 퍼옥사이드
        {
            double mass = double.Parse(textBox1.Text);
            hydrogen1 = mass;
            oxygen = mass * 8; // 수소 : 산소 = 1 : 8 로 퍼옥사이드 1 합성
            dataGridView2.Rows[0].Cells[1].Value = hydrogen1 + hydrogen2; // 수소 양
            dataGridView2.Rows[2].Cells[1].Value = oxygen;   // 산소 양
        }
        
        private void textBox2_TextChanged(object sender, EventArgs e) // 하이드라진
        {
            double mass = double.Parse(textBox2.Text);
            hydrogen2 = mass;
            nitrogen = mass * 7; // 수소 : 질소 = 1 : 7 로 하이드라진 1 합성
            dataGridView2.Rows[0].Cells[1].Value = hydrogen1 + hydrogen2; // 수소 양
            dataGridView2.Rows[1].Cells[1].Value = nitrogen; // 질소 양
        }

        private void textBox3_TextChanged(object sender, EventArgs e) // 엑실론
        {
            double mass = double.Parse(textBox3.Text);
            epsilonCrystal = mass * 2; // 엑실론-크리스탈 2로 엑실론 1 합성
            dataGridView2.Rows[3].Cells[1].Value = epsilonCrystal; // 엑실론-크리스탈 양
        }

        // TODO: 아이템 합성 구현
        // TODO: Enter 누르면 아이템 합성, 합성 시 확인창으로 합성 확인
        // TODO: 선택 된 아이템 없으면 합성 버튼 비활성화
        // TODO: 선택한 아이템, 개수가 보유 자원으로 합성 불가능하면 합성 버튼 비활성화
        // TODO: 아이템 개수 입력 시 필요 자원 띄워주기

    }
}