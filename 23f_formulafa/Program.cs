using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace _23f_formulafa
{
	internal class Program
	{
		class Formula
		{
			public List<Formula> gyerekei;
			public char művelet; // nem csak ∧ ∨ ¬ → ↔, de a p, q, r, ... is művelet lesz!




			public Formula(char művelet)
			{
				this.művelet = művelet;
				this.gyerekei = new List<Formula>();
			}

			public Formula(char művelet, List<Formula>gyerekei)
			{
				this.művelet = művelet;
				this.gyerekei = gyerekei;
			}

			public bool Ugyanaz(Formula f2)
			{
				bool result = this.művelet == f2.művelet && this.gyerekei.Count == f2.gyerekei.Count;
				int i = 0;
				while(i < this.gyerekei.Count && result)
				{
					result &= this.gyerekei[i].Ugyanaz(f2.gyerekei[i]);
					i++;
				}
				return result;
			}

			public static Formula operator *(Formula A, Formula B) => new Formula('&', new List<Formula> { A, B });
			public static Formula operator +(Formula A, Formula B) => new Formula('V', new List<Formula> { A, B });
			public static Formula operator -(Formula A) => new Formula('¬', new List<Formula> { A });
			public static Formula operator >(Formula A, Formula B) => new Formula('→', new List<Formula> { A, B });
			public static Formula operator <(Formula A, Formula B) => new Formula('←', new List<Formula> { A, B });
			public static Formula operator ==(Formula A, Formula B) => new Formula('↔', new List<Formula> { A, B });
			public static Formula operator !=(Formula A, Formula B) => new Formula('⨂', new List<Formula> { A, B });
			public override string ToString()
			{
				switch (this.gyerekei.Count)
				{
					case 0: // ez egy atomi formula
						return this.művelet.ToString();
					case 1: // ez egy tagadás
						return this.művelet.ToString() + this.gyerekei[0].ToString();
					case 2: // ez egy és, vagy, ...
						return "(" + this.gyerekei[0].ToString() + this.művelet.ToString() + this.gyerekei[1].ToString() + ")";
					default:
						throw new NotImplementedException();
				}
			}  

			int Mélység()
			{
				int max = -1;
				foreach (Formula gyerek in gyerekei)
				{
					int m = gyerek.Mélység();
					if (max<m)
					{
						max = m; 
					}
				}
				return max + 1;
			}

			HashSet<Formula> Atomi_formulai()
			{
				HashSet<Formula> atoms = new HashSet<Formula>();

				if (gyerekei.Count == 0)
				{
					atoms.Add(this);
					return atoms;
				}

				foreach (Formula gyerek in gyerekei)
				{
					atoms.UnionWith(gyerek.Atomi_formulai());
				}

				return atoms;
			}

			/// <summary>
			/// Megmondja, hogy a megadott műveletből hányat tartalmaz a formula.
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			public int Műveletek_száma(char operátor)
			{
				if (gyerekei.Count == 0)
					return 0;

				int sum = 0;
				foreach (Formula gyerek in gyerekei)
				{
					sum += gyerek.Műveletek_száma(operátor);
				}

				if (művelet == operátor)
				{
					return 1 + sum;
				}
								
				return sum;
			}

			public bool Atomi() => this.gyerekei.Count == 0;

			public Formula NemÉs()
			{
				if (this.Atomi())
					return this;
				if (this.művelet == '¬')
					return -this.gyerekei[0].NemÉs();
				if (this.művelet == '&')
					return this.gyerekei[0].NemÉs() * this.gyerekei[1].NemÉs();
				if (this.művelet == 'V')
					return -(-this.gyerekei[0].NemÉs() * -this.gyerekei[1].NemÉs());
				if (this.művelet == '→')
					return -(this.gyerekei[0].NemÉs() * -this.gyerekei[1].NemÉs());
				if (this.művelet == '↔')
					return this.gyerekei[0].NemÉs() > this.gyerekei[1].NemÉs() * this.gyerekei[1].NemÉs() > this.gyerekei[0].NemÉs();
				if (this.művelet == '⨂')
					return -(this.gyerekei[0].NemÉs() == this.gyerekei[1].NemÉs());
				return null;
			}

			/// <summary>
			/// Literálnak nevezzük az atomi formulákat vagy azok tagadásait.
			/// </summary>
			/// <returns></returns>
			public bool Literál() => Atomi() || (művelet == '¬' && this.gyerekei[0].Atomi());

			/// <summary>
			/// Visszaadja, hogy hányszor tagadták benne a megadott műveletet. Például
			/// Tagadott('&') azt adja vissza, hogy hányszor volt konjunkció tagadva benne.
			/// Tagadott('V') azt adja vissza, hogy hányszor volt diszjunkció tagadva benne.
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			//int Tagadott(char muvelet)
			//{

			//}

			/// <summary>
			/// Elkészíti a formula kettős tagadások nélküli verzióját.
			/// </summary>
			/// <returns></returns>
			//Formula Kettostagadasok_nelkul()
			//{

			//}

			/// <summary>
			/// Diszjunktív normálformára hoz, azaz megadja azt az ekvivalens átalakítását, amiben csak V műveletek és literálok vannak. 
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			//Formula Diszjunktív_normálforma()
			//{

			//}

			/// <summary>
			/// Konjunktív normálformára hoz.
			/// </summary>
			/// <param name="operátor"></param>
			/// <returns></returns>
			//Formula Konjunktív_normálforma()
			//{

			//}

			public static bool Kielégíthető(HashSet<Formula> formulahalmaz)
			{
				HashSet<Formula> literálok = new HashSet<Formula>();
				Stack<Formula> formulaverem = new Stack<Formula>();
				foreach (Formula formula in formulahalmaz)
				{
					formulaverem.Push(formula.NemÉs());
					if (formula.Literál())
					{
						literálok.Add(formula);
					}
				}

				Analitikus_fa fa = new Analitikus_fa(formulaverem, literálok);
				return fa.kielégíthető;
			}

			public static bool Ellentmondásos(HashSet<Formula> formulahalmaz) => !Kielégíthető(formulahalmaz);

			public static bool Logikai_igazság(Formula formula) => Ellentmondásos(new HashSet<Formula> { -formula });

		}

		class Analitikus_fa
		{
			public bool kielégíthető;
			public bool ezazágkielégíthető;
			public List<Analitikus_fa> gyerekei;
			public Stack<Formula> gyökér;
			public override string ToString()
			{
				List<string> labels = new List<string>();
				string result = this.ToStringVáz(labels);
				for (int i = 0; i < labels.Count; i++)
				{
					result += $"\n{i}[label=\"{labels[i]}\"];";
				}
				return result;
			}
			private string ToStringVáz(List<string> labels)
			{
                string s = "";
                //string gyokerstr = string.Join("\\n", gyökér);

				int id = labels.Count;
				bool elsociklus = true;
                foreach (Analitikus_fa gyerek in this.gyerekei)
                {
					string következő;
					if (gyerek.gyerekei.Count == 0)
					{
						következő = $"{labels.Count};\n";
						labels.Add(gyerek.gyökér.Pop() + "\\n" + (ezazágkielégíthető ? "O" : "*"));
					}
					else
					{
						következő = gyerek.ToStringVáz(labels);
					}
					if (elsociklus)
					{
						id = labels.Count;
						elsociklus = false;
						labels.Add(string.Join("\\n", gyökér));
					}
                    s += $"{id} -> {következő}";
                }

                return s;
            }

			public Analitikus_fa(Stack<Formula> formulahalmaz, HashSet<Formula> literálok)
			{
				this.gyökér = new Stack<Formula>(formulahalmaz); // kell-e?
				this.gyökér = new Stack<Formula>(this.gyökér);
                this.gyerekei = new List<Analitikus_fa> { };
				HashSet<Formula> továbbadott_literálok = new HashSet<Formula>(literálok);

				Stack<Formula> formulahalmaz_másolat = new Stack<Formula>(this.gyökér);
                HashSet<Formula> összes_literál = new HashSet<Formula>(literálok);
				foreach (Formula formula in formulahalmaz_másolat)
				{
					if(formula.Literál()) összes_literál.Add(formula);
				}
                ezazágkielégíthető = true;
				foreach (Formula literál in összes_literál)
				{
					foreach (Formula pár in összes_literál)
					{
						ezazágkielégíthető &= !(literál.Ugyanaz(-pár) || pár.Ugyanaz(-literál));
                    }
				}

				if (formulahalmaz.Count == 0)
				{
					this.kielégíthető = false;
					return;
				}

				Formula teteje = formulahalmaz.Pop();

				// összes lehetőség, tekintve, hogy ide csak akkor jutunk, ha már &-re és tagadásra átírtuk az egész formulát!
				// - atomi formulával van dolgunk
				// - tagadott valamilyen formulával van dolgunk
				//   - tagadott atomi formula
				//   - duplán tagadott formula
				//   - tagadott &-es formula
				// - &-es formula

				if (teteje.Atomi())
				{
					// ha atomi formulával állunk szemben
					
					if (összes_literál.Contains(-teteje)) // ha találunk ellentmondást, akkor vége a kisebb fákra való szétbontásnak.
					{
						this.kielégíthető = false;
                    }
					else if (formulahalmaz.Count != 0)
					{
						Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
						kov_formulahalmaz = new Stack<Formula>(kov_formulahalmaz);
						továbbadott_literálok.Add(teteje);
						this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));
						this.kielégíthető = this.gyerekei[0].kielégíthető;
					}
					else this.kielégíthető = this.ezazágkielégíthető;

				}
				else if (teteje.művelet=='¬')
				{
					// ha tagadott valamilyen formulával állunk szemben... lásd lejjebb
					Formula gyerek = teteje.gyerekei[0];

					if (gyerek.Atomi())
					{
						// ha tagadott atomi formulával állunk szemben: "-p"
						if (összes_literál.Contains(gyerek))  // ezt most nem kell tagadni, mert teteje az, ami tagadott, tehát ha annak a gyereke ott van a literálhalmazban, akkor a teteje ellentmond neki!
						{
							this.kielégíthető = false;
						}
						else if (formulahalmaz.Count != 0)
						{
							Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
							továbbadott_literálok.Add(teteje);
							this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));
							this.kielégíthető = this.gyerekei[0].kielégíthető;
						}
						else this.kielégíthető = this.ezazágkielégíthető;
					}
					else if (gyerek.művelet == '¬')
					{
						// ha a tagadáson belül tagadás van: "--p" származékai

						Formula unoka = gyerek.gyerekei[0];
						Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
						kov_formulahalmaz.Push(unoka);
						this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));
						this.kielégíthető = this.gyerekei[0].kielégíthető;
					}
					else if (gyerek.művelet == '&')
					{
						// ha a tagadáson belül & van: "-(p&q)" származékai
						Formula balunoka = -gyerek.gyerekei[0];
						Formula jobbunoka = -gyerek.gyerekei[1];

						Stack<Formula> bal_formulahalmaz = new Stack<Formula>(formulahalmaz);
						bal_formulahalmaz.Push(balunoka);
						
						Stack<Formula> jobb_formulahalmaz = new Stack<Formula>(formulahalmaz);
						jobb_formulahalmaz.Push(jobbunoka);

                        this.gyerekei.Add(new Analitikus_fa(bal_formulahalmaz, továbbadott_literálok));
						this.gyerekei.Add(new Analitikus_fa(jobb_formulahalmaz, továbbadott_literálok));
						this.kielégíthető = this.gyerekei[0].kielégíthető || this.gyerekei[1].kielégíthető;
					}

				}
				else if (teteje.művelet == '&')
				{
					// Ha konjunkcióval állunk szemben
					Formula bal = teteje.gyerekei[0];
					Formula jobb = teteje.gyerekei[1];

					Stack<Formula> kov_formulahalmaz = new Stack<Formula>(formulahalmaz);
					kov_formulahalmaz.Push(bal);
					kov_formulahalmaz.Push(jobb);

					this.gyerekei.Add(new Analitikus_fa(kov_formulahalmaz, továbbadott_literálok));

					this.kielégíthető = this.gyerekei[0].kielégíthető;
				}
				// több lehetőség nincs, mert ez az analitikus fa úgy hívódik majd meg, hogy a formula csak tagadást és konjunkciót tartalmaz majd.

			}
		}



		static void Main(string[] args)
		{
			Formula p = new Formula('p');
			Formula q = new Formula('q');
			Formula p_es_q = p * q;
			Formula A = ((p * q) + -p) > q;

			//Console.WriteLine(A);

			Formula r = new Formula('r');
			
			//rajzunk:
			Formula B = -(-p > (q * r))+p;

			Formula C = p * (q + r);

			Formula D = -p * p;

			Formula E = p + (q * -q);

			Formula F = -(p * q);

			Stack<Formula> stck = new Stack<Formula>();
			//stck.Push(p);
			//stck.Push(q);
			//stck.Push(r);
			stck.Push(A.NemÉs());
			stck.Push(B.NemÉs());
			stck.Push(C.NemÉs());
			//stck.Push(D.NemÉs());
			//stck.Push(E.NemÉs());
			//stck.Push(F.NemÉs());
            Analitikus_fa fa = new Analitikus_fa(stck, new HashSet<Formula>());

            //Console.WriteLine("digraph G {");
            Console.WriteLine(fa.ToString());
            //Console.WriteLine("}");
            //Console.ReadLine();

            //Console.WriteLine(fa.kielégíthető);

        }
	}
}
