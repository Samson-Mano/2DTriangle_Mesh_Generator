using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _2DTriangle_Mesh_Generator.txt_input_reader
{
    public class txt_rd_reader
    {
        // Text reader is used to read raw data (.txt) output from VARAI2D surface

        public bool is_read_success;
        public List<string_data_store> str_surf_datas { get; private set; }
        public List<string_data_store> str_bezier_datas { get; private set; }
        public List<string_data_store> str_arc_datas { get; private set; }
        public List<string_data_store> str_line_datas { get; private set; }
        public List<string_data_store> str_endpt_datas { get; private set; }

        public struct string_data_store
        {
            public string str_id;
            public string str_main_data;
            public List<string> str_additional_data;

            public string_data_store(string t_str_id, string t_str_main_data, List<string> t_str_additional_data)
            {
                str_id = t_str_id;
                str_main_data = t_str_main_data;
                str_additional_data = new List<string>(t_str_additional_data);
            }

            public string output_check()
            {
                string out_pt = "";

                // Id output
                out_pt = "Id = " + str_id + Environment.NewLine;

                // Main data
                out_pt = out_pt + "Main data = " + str_main_data + Environment.NewLine;

                // Additional data
                int i = 1;
                foreach (string st in str_additional_data)
                {
                    out_pt = out_pt + "Additional data " + i.ToString() + " = " + st + Environment.NewLine;
                    i++;
                }

                return out_pt;
            }
        }

        public string txt_reader_ouput()
        {
            string outpt_rslt = "";
            if (is_read_success == true)
            {
                outpt_rslt =  "________________________________________" + Environment.NewLine; 
                outpt_rslt = outpt_rslt + "Surfaces" + Environment.NewLine;
                foreach (txt_rd_reader.string_data_store rslt in str_surf_datas)
                {
                    outpt_rslt = outpt_rslt + rslt.output_check();
                }

                outpt_rslt = outpt_rslt + "________________________________________" + Environment.NewLine;
                outpt_rslt = outpt_rslt + "Bezier Curves" + Environment.NewLine;
                foreach (txt_rd_reader.string_data_store rslt in str_bezier_datas)
                {
                    outpt_rslt = outpt_rslt + rslt.output_check();
                }

                outpt_rslt = outpt_rslt + "________________________________________" + Environment.NewLine;
                outpt_rslt = outpt_rslt + "Arcs " + Environment.NewLine;
                foreach (txt_rd_reader.string_data_store rslt in str_arc_datas)
                {
                    outpt_rslt = outpt_rslt + rslt.output_check();
                }

                outpt_rslt = outpt_rslt + "________________________________________" + Environment.NewLine;
                outpt_rslt = outpt_rslt + "Lines " + Environment.NewLine;
                foreach (txt_rd_reader.string_data_store rslt in str_line_datas)
                {
                    outpt_rslt = outpt_rslt + rslt.output_check();
                }

                outpt_rslt = outpt_rslt + "________________________________________" + Environment.NewLine;
                outpt_rslt = outpt_rslt + "End Points " + Environment.NewLine;
                foreach (txt_rd_reader.string_data_store rslt in str_endpt_datas)
                {
                    outpt_rslt = outpt_rslt + rslt.output_check();
                }
            }

            return outpt_rslt;
        }

        public txt_rd_reader(string file_name)
        {
            this.is_read_success = false;
            try
            {
                // Initialize the data store
                str_surf_datas = new List<string_data_store>();
                str_bezier_datas = new List<string_data_store>();
                str_arc_datas = new List<string_data_store>();
                str_line_datas = new List<string_data_store>();
                str_endpt_datas = new List<string_data_store>();

                string[] split_str;
                string temp_str_data_id = "";
                string temp_str_main_data = "";
                List<string> temp_str_addl_data = new List<string>();

                int i = 0, j = 0;
                // Stream reader to read line
                using (StreamReader sreader = File.OpenText(file_name))
                {
                    string str_ln = String.Empty;
                    while ((str_ln = sreader.ReadLine()) != null)
                    {
                        // read line by line
                        if (str_ln.Substring(0, 3) == "[+]")
                        {
                            string[] str_elem_type = str_ln.Substring(4).Split(',');

                            // store the number of elements
                            int n_count = 0;
                            int.TryParse(str_elem_type[1], out n_count);

                            if (str_elem_type[0] == "End Points")
                            {
                                // Read all End points
                                for (i = 0; i < n_count; i++)
                                {
                                    // Split the end point data to the components
                                    split_str = sreader.ReadLine().Split(',');
                                    temp_str_data_id = split_str[0];
                                    temp_str_main_data = split_str[1] + ", " + split_str[2];
                                    temp_str_addl_data = new List<string>();

                                    // Add to the data to the endpoint string list
                                    str_endpt_datas.Add(new string_data_store(temp_str_data_id,
                                        temp_str_main_data,
                                        temp_str_addl_data));
                                }
                            }
                            else if (str_elem_type[0] == "Lines")
                            {
                                // Read all Lines
                                for (i = 0; i < n_count; i++)
                                {
                                    // Split the lines data to the components
                                    split_str = sreader.ReadLine().Split(',');
                                    temp_str_data_id = split_str[0];
                                    temp_str_main_data = split_str[1] + ", " + split_str[2];
                                    temp_str_addl_data = new List<string>();

                                    // Add to the data to the lines string list
                                    str_line_datas.Add(new string_data_store(temp_str_data_id,
                                        temp_str_main_data,
                                        temp_str_addl_data));
                                }
                            }
                            else if (str_elem_type[0] == "Arcs")
                            {
                                // Read all Arcs
                                for (i = 0; i < n_count; i++)
                                {
                                    // Each arc has 3 lines of inputs
                                    split_str = sreader.ReadLine().Split(',');
                                    temp_str_data_id = split_str[0];
                                    temp_str_main_data = split_str[1] + ", " + split_str[2];
                                    temp_str_addl_data = new List<string>();

                                    for (j = 0; j < 2; j++)
                                    {
                                        // Additional crown and centerpt is added
                                        split_str = sreader.ReadLine().Split(',');
                                        temp_str_addl_data.Add(split_str[1] + ", " + split_str[2]);
                                    }

                                    // Add to the data to the arcs string list
                                    str_arc_datas.Add(new string_data_store(temp_str_data_id,
                                        temp_str_main_data,
                                        temp_str_addl_data));
                                }

                            }
                            else if (str_elem_type[0] == "Beziers")
                            {
                                // Read all Beziers
                                for (i = 0; i < n_count; i++)
                                {
                                    // Each beziers has 1,2 or 3 control points (2 end points)
                                    string str_temp = sreader.ReadLine();
                                    int index0 = str_temp.IndexOf('@');

                                    // Find the cntrl point length
                                    int cntrl_count = 0;
                                    int.TryParse(str_temp.Substring(index0 + 1), out cntrl_count);

                                    // First data
                                    split_str = str_temp.Split(',');
                                    temp_str_data_id = split_str[0];
                                    temp_str_main_data = split_str[1] + ", " + split_str[2];
                                    temp_str_addl_data = new List<string>();

                                    for (j = 0; j < cntrl_count; j++)
                                    {
                                        // Additional control point is added
                                        split_str = sreader.ReadLine().Split(',');
                                        temp_str_addl_data.Add(split_str[1] + ", " + split_str[2]);
                                    }

                                    // Add to the data to the bezier string list
                                    str_bezier_datas.Add(new string_data_store(temp_str_data_id,
                                        temp_str_main_data,
                                        temp_str_addl_data));
                                }
                            }
                            else if (str_elem_type[0] == "Surfaces")
                            {
                                // Read all surfaces
                                for (i = 0; i < n_count; i++)
                                {
                                    string str_temp = sreader.ReadLine();
                                    int index0 = str_temp.IndexOf('{');
                                    int index1 = str_temp.IndexOf('}');

                                    temp_str_data_id = str_temp.Substring(0, index0 - 2);

                                    // remove the last 3 characters
                                    temp_str_main_data = str_temp.Substring(index0 + 1, index1 - index0-1);

                                    index0 = str_temp.IndexOf('@');
                                    int cntrl_count = 0;
                                    int.TryParse(str_temp.Substring(index0 + 1), out cntrl_count);
                                    temp_str_addl_data = new List<string>();

                                    for (j = 0; j < cntrl_count; j++)
                                    {
                                        // Additional nested surface added
                                        str_temp = sreader.ReadLine();
                                        index0 = str_temp.IndexOf('[');
                                        index1 = str_temp.IndexOf(']');

                                        temp_str_addl_data.Add(str_temp.Substring(index0 + 1, index1 - index0 - 1));
                                    }

                                    // Add to the data to the bezier string list
                                    str_surf_datas.Add(new string_data_store(temp_str_data_id,
                                        temp_str_main_data,
                                        temp_str_addl_data));
                                }
                            }
                        }
                    }
                }


                this.is_read_success = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry!!!!! Unable to Open.. File Reading Error" + ex.Message, "Samson Mano", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
