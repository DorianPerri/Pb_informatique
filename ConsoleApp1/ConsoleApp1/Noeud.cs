using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Noeud
    {
        public int id;
        public Noeud(int ide) { 
            this.id = ide;
        } 
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
    }
}
