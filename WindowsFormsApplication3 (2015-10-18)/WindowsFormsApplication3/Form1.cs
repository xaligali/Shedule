using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        List<Vertex> vertices;//список вершин
        List<Edge> edges;//список дуг
        List<TimeTable> timeTables;//список расписаний на день
        Vertex currentVertex;//текущая вершина
        Edge currentEdge;//текущая дуга
        TimeTable currentTimeTable;//текущее расписание
        bool vertexSelection;//выбрана ли вершина (true - да, false - нет)
        int numbVertex;//число вершины
        int numbEdge;//число дуг
        int numbTimeTable;//число расписаний на день
        int[,] A;//матрица расстояний (между всеми вершинами)

        public Form1()
        {
            InitializeComponent();
            try
            {
                vertices = new List<Vertex>() { };
                edges = new List<Edge>() { };
                timeTables = new List<TimeTable>() { };
                currentVertex = new Vertex(0);
                currentEdge = new Edge(0);
                currentTimeTable = new TimeTable(0);
                vertexSelection = false;
                numbVertex = 0;
                numbEdge = 0;
                numbTimeTable = 0;

                FormObjectsResize();
                GraphVisualization();
            }
            catch { }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                FormObjectsResize();
                GraphVisualization();
            }
            catch { }
        }

        private void FormObjectsResize()
        {
            int formSizeWidth = this.Size.Width;
            int formSizeHeight = this.Size.Height;
            int constantSize = (formSizeWidth - 40) / 2;

            int pictureBoxSizeWidth = constantSize;
            int pictureBoxSizeHeight = constantSize;
            int richTextBoxSizeWidth = constantSize;
            int richTextBoxSizeHeight = constantSize;
            int groupBoxSizeWidth = 465;
            int groupBoxSizeHeight = 100;

            int pictureBoxLocationX = 10;
            int pictureBoxLocationY = 10;
            int richTextBoxLocationX = 15 + constantSize;
            int richTextBoxLocationY = 10;
            int groupBoxLocationX = 10;
            int groupBoxLocationY = 15 + constantSize;

            pictureBox1.Size = new Size(pictureBoxSizeWidth, pictureBoxSizeHeight);
            richTextBox1.Size = new Size(richTextBoxSizeWidth, richTextBoxSizeHeight);
            groupBox1.Size = new Size(groupBoxSizeWidth, groupBoxSizeHeight);
            pictureBox1.Location = new Point(pictureBoxLocationX, pictureBoxLocationY);
            richTextBox1.Location = new Point(richTextBoxLocationX, richTextBoxLocationY);
            groupBox1.Location = new Point(groupBoxLocationX, groupBoxLocationY);
        }

        private void GraphVisualization()
        {
            try
            {
                Bitmap bitmap = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.Gainsboro);

                foreach (Edge ed in edges)
                {
                    ed.EdgeDrawing(graphics, Color.Black);
                }
                foreach (Vertex ve in vertices)
                {
                    if (ve.vertexID == 0)
                    {
                        ve.RectangleVertexDrawing(graphics);
                        ve.RectangleBorderDrawing(graphics, Color.Black);
                    }
                    else
                    {
                        ve.EllipseVertexDrawing(graphics);
                        ve.EllipseBorderDrawing(graphics, Color.Black);
                    }
                }
                if (vertexSelection == true)
                {
                    if (currentVertex.vertexID == 0)
                    {
                        currentVertex.RectangleVertexDrawing(graphics);
                        currentVertex.RectangleBorderDrawing(graphics, Color.Orange);
                    }
                    else
                    {
                        currentVertex.EllipseVertexDrawing(graphics);
                        currentVertex.EllipseBorderDrawing(graphics, Color.Orange);
                    }
                }

                pictureBox1.Image = bitmap;
            }
            catch
            {
                MessageBox.Show("Ошибка при клике по pictureBox.", "Сообщение");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                A = new int[numbVertex, numbVertex];
                for (int i = 0; i < numbVertex; i++)
                {
                    for (int j = 0; j < numbVertex; j++)
                    {
                        A[i, j] = 0;
                    }
                }
                foreach (Edge ed in edges)
                {
                    //вопрос: порядковый номер вершины в списке и его id - совпадают ли?
                    A[ed.vertexAID, ed.vertexBID] = ed.price;
                    A[ed.vertexBID, ed.vertexAID] = ed.price;
                }
                richTextBox1.Text = "Матрица изначальная:\n";
                for (int i = 0; i < numbVertex; i++)
                {
                    for (int j = 0; j < numbVertex; j++)
                    {
                        richTextBox1.Text += A[i, j].ToString() + " ";
                    }
                    richTextBox1.Text += "\n";
                }

                //проверка на связность графа
                for (int l = 0; l < numbVertex; l++)
                {
                    for (int i = 0; i < numbVertex; i++)
                    {
                        for (int j = 0; j < numbVertex; j++)
                        {
                            if (i != j)
                            {
                                for (int k = 0; k < numbVertex; k++)
                                {
                                    if ((k != i) && (k != j))
                                    {
                                        if ((A[i, k] != 0) && (A[k, j] != 0) && (((A[i, j] != 0) && (A[i, k] + A[k, j] < A[i, j])) || (A[i, j] == 0)))
                                        {
                                            A[i, j] = A[i, k] + A[k, j];
                                            A[j, i] = A[i, j];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                int S = 0;
                for (int i = 0; i < numbVertex; i++)
                {
                    for (int j = 0; j < numbVertex; j++)
                    {
                        if (A[i, j] == 0)
                        {
                            S += 1;
                        }
                    }
                }
                richTextBox1.Text += "Матрица после преобразования:\n";
                for (int i = 0; i < numbVertex; i++)
                {
                    for (int j = 0; j < numbVertex; j++)
                    {
                        richTextBox1.Text += A[i, j].ToString() + " ";
                    }
                    richTextBox1.Text += "\n";
                }
                if (S > numbVertex)
                {
                    richTextBox1.Text += "Граф несвязен";
                }
                else
                {
                    richTextBox1.Text += "Граф связен";
                }
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //выбираем критические вершины и упорядочиваем их в порядке уменьшения важности
                currentTimeTable = new TimeTable(numbTimeTable);
                int constantTotalTime = 480;//пусть 8 часов
                int currentTotalTime = 0;
                List<Vertex> importantVertices = new List<Vertex>() { };
                List<Vertex> routVertices = new List<Vertex>() { };
                foreach (Vertex ve in vertices)
                {
                    if ((ve.supply <= 0) && (ve.vertexID != 0))
                    {
                        importantVertices.Add(ve);
                    }
                }
                //выбираем первую вершину их списка важных вершины и пытаемся посетить остальные
                int currentNumb = 0;
                if (importantVertices.Count > 0)
                {
                    routVertices.Add(vertices[0]);
                    currentNumb += 1;
                    foreach (Vertex ve in importantVertices)
                    {
                        if (currentTotalTime + A[routVertices[currentNumb - 1].vertexID, ve.vertexID] + ve.duration + A[ve.vertexID, 0] <= constantTotalTime)
                        {
                            routVertices.Add(ve);
                            currentNumb += 1;
                        }
                    }
                    routVertices.Add(vertices[0]);
                    currentNumb += 1;
                }
                //посещаем выбранные вершины
                for (int i = 0; i < routVertices.Count; i++)
                {
                    currentTimeTable.vertices.Add(routVertices[i]);
                    routVertices[i].DayWithVisit();
                }
                timeTables.Add(currentTimeTable);
                numbTimeTable += 1;
                //проходит день
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (vertices[i].vertexID != 0)
                    {
                        vertices[i].DayWithoutVisit();
                    }
                }
                //выводим результат
                richTextBox1.Text += "\nДень: " + currentTimeTable.timeTableID.ToString() + ".\n";
                foreach (Vertex ve in currentTimeTable.vertices)
                {
                    richTextBox1.Text += ve.vertexID.ToString() + " ";
                }
                richTextBox1.Text += "\n";

                FormObjectsResize();
                GraphVisualization();
            }
            catch
            {
                richTextBox1.Text = "Ошибка";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch { }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dataSet = new DataSet();
                if (File.Exists("table.xml"))
                {
                    dataSet.ReadXml("table.xml");
                    richTextBox1.Text = "";
                    richTextBox1.Text += "Число таблиц: " + dataSet.Tables.Count.ToString() + "\n";
                    richTextBox1.Text += "\n";
                    for (int i = 0; i < dataSet.Tables.Count; i++)
                    {
                        richTextBox1.Text += "Имя таблицы: " + dataSet.Tables[i].Namespace.ToString() + " " + i +"\n";
                        richTextBox1.Text += "Число строк: " + dataSet.Tables[i].Rows.Count.ToString() + "\n";
                        richTextBox1.Text += "Число столбцов: " + dataSet.Tables[i].Columns.Count.ToString() + "\n";
                        richTextBox1.Text += "\n";
                    }
                    foreach (DataTable dt in dataSet.Tables)
                    {
                        richTextBox1.Text += "Имя таблицы: " + dt.Namespace.ToString() + " " + "\n";
                        richTextBox1.Text += "Число строк: " + dt.Rows.Count.ToString() + "\n";
                        richTextBox1.Text += "Число столбцов: " + dt.Columns.Count.ToString() + "\n";
                        richTextBox1.Text += "\n";
                    }
                    MessageBox.Show("Загружено");
                }
                else
                {
                    MessageBox.Show("Файла нет");
                }
                //DataTable dataTable = new DataTable();
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Graph serializableObject = new Graph();
                serializableObject.vertices = vertices;
                serializableObject.edges = edges;
                serializableObject.verticesNumber = numbVertex;
                serializableObject.edgesNumber = numbEdge;
                Serializer serializer = new Serializer();
                serializer.ObjectSerialize("file.save", serializableObject);
                MessageBox.Show("Данные успешно сохранены.", "Сообщение");
            }
            catch
            {
                MessageBox.Show("Ошибка при сохранении в файл.", "Сообщение");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                Graph serializableObject = new Graph();
                Serializer serializer = new Serializer();
                serializableObject = serializer.ObjectDeserialize("file.save");
                vertices = serializableObject.vertices;
                edges = serializableObject.edges;
                numbVertex = serializableObject.verticesNumber;
                numbEdge = serializableObject.edgesNumber;
                GraphVisualization();
                MessageBox.Show("Данные успешно считаны.", "Сообщение");
            }
            catch
            {
                MessageBox.Show("Ошибка при считывании из файла.", "Сообщение");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch
            {
                MessageBox.Show("Ошибка при закрытии приложения.", "Сообщение");
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                FormObjectsResize();

                Point point = e.Location;//место клика мыши
                bool imposition = false;//наложение вершины на другую вершину
                bool uniqueEdge = true;//уникальность дуги (true - дуга уникальна , false - дуга не уникальна)

                //выбор вершины
                if (radioButton1.Checked == true)
                {
                    foreach (Vertex ve in vertices)
                    {
                        if ((int)Math.Pow((double)((ve.point.X - point.X) * (ve.point.X - point.X) + (ve.point.Y - point.Y) * (ve.point.Y - point.Y)), 0.5) <= 6)
                        {
                            currentVertex = new Vertex(ve.vertexID, ve.point.X, ve.point.Y, ve.duration, ve.period, ve.supply);
                            richTextBox1.Text = "Вершина выбрана:\n";
                            currentVertex.WriteFromVertexToRichTextBox(richTextBox1);
                            break;
                        }
                    }
                }

                //добавление вершины
                if (radioButton2.Checked == true)
                {
                    foreach (Vertex ve in vertices)
                    {
                        if ((int)Math.Pow((double)((ve.point.X - point.X) * (ve.point.X - point.X) + (ve.point.Y - point.Y) * (ve.point.Y - point.Y)), 0.5) <= 10)
                        {
                            imposition = true;
                            richTextBox1.Text = "Наложение вершин.";
                            break;
                        }
                    }

                    if (imposition == false)
                    {
                        Random random = new Random();
                        if (numbVertex == 0)
                        {
                            currentVertex = new Vertex(0, point.X, point.Y, 0, 0, 0);
                        }
                        else
                        {
                            currentVertex = new Vertex(numbVertex, point.X, point.Y, 20, 7, random.Next(0, 7));
                        }
                        vertices.Add(currentVertex);
                        numbVertex += 1;

                        richTextBox1.Text = "Вершина добавлена:\n";
                        currentVertex.WriteFromVertexToRichTextBox(richTextBox1);
                    }
                }

                //добавление дуги
                if (radioButton3.Checked == true)
                {
                    foreach (Vertex ve in vertices)
                    {
                        if ((int)Math.Pow((double)((ve.point.X - point.X) * (ve.point.X - point.X) + (ve.point.Y - point.Y) * (ve.point.Y - point.Y)), 0.5) <= 6)
                        {
                            if (vertexSelection == false)
                            {
                                richTextBox1.Text = ve.vertexID.ToString();
                                currentVertex = new Vertex(ve.vertexID, ve.point.X, ve.point.Y, ve.duration, ve.period, ve.supply);
                                vertexSelection = true;
                            }
                            else
                            {
                                //проверка на наличие дуги в списке дуг
                                foreach (Edge ed in edges)
                                {
                                    if ((ed.pointA.X == currentVertex.point.X) && (ed.pointA.Y == currentVertex.point.Y) && (ed.pointB.X == ve.point.X) && (ed.pointB.Y == ve.point.Y))
                                    {
                                        uniqueEdge = false;
                                        richTextBox1.Text = "Совпадение дуг.\n";
                                        richTextBox1.Text += "Всего дуг: " + edges.Count.ToString();
                                        break;
                                    }
                                    if ((ed.pointA.X == ve.point.X) && (ed.pointA.Y == ve.point.Y) && (ed.pointB.X == currentVertex.point.X) && (ed.pointB.Y == currentVertex.point.Y))
                                    {
                                        uniqueEdge = false;
                                        richTextBox1.Text = "Совпадение дуг.\n";
                                        richTextBox1.Text += "Всего дуг: " + edges.Count.ToString();
                                        break;
                                    }
                                }

                                //добавление дуги
                                if ((ve.vertexID != currentVertex.vertexID) && (uniqueEdge == true))
                                {
                                    Random random = new Random();
                                    currentEdge = new Edge(numbEdge, currentVertex.vertexID, ve.vertexID, currentVertex.point, ve.point, 5 * random.Next(2, 5));
                                    edges.Add(currentEdge);
                                    numbEdge += 1;
                                    richTextBox1.Text = "Дуга добавлена.\n";
                                    richTextBox1.Text += "Всего дуг: " + edges.Count.ToString();
                                }
                                vertexSelection = false;
                            }
                            break;
                        }
                    }   
                }
                GraphVisualization();
            }
            catch
            {
                MessageBox.Show("Ошибка при клике по pictureBox.", "Сообщение");
            }
        }
    }

    //класс "Вершина"
    [Serializable]
    public class Vertex
    {
        //свойства
        public int vertexID;//уникальный номер вершины
        public Point point;//координаты вершины
        public int supply;//текущий запас вершины (может быть отрицательным)
        public int period;//срок ("моп")
        public Color color;//текущий цвет вершины
        public int duration;//продолжительность посещения вершины
        public int visiting;//время посещения вершины

        //методы
        public void RectangleVertexDrawing(Graphics graphics)
        {
            Point newPoint = new Point(point.X - 5, point.Y - 5);
            Size size = new Size(10, 10);
            Rectangle rectangle = new Rectangle(newPoint, size);
            graphics.FillRectangle(new SolidBrush(color), rectangle);
        }

        public void RectangleBorderDrawing(Graphics graphics, Color borderColor)
        {
            Point newPoint = new Point(point.X - 5, point.Y - 5);
            Size size = new Size(10, 10);
            Rectangle rectangle = new Rectangle(newPoint, size);
            graphics.DrawRectangle(new Pen(borderColor, 3.0f), rectangle);
        }

        public void EllipseVertexDrawing(Graphics graphics)
        {
            Point newPoint = new Point(point.X - 5, point.Y - 5);
            Size size = new Size(10, 10);
            Rectangle rectangle = new Rectangle(newPoint, size);
            graphics.FillEllipse(new SolidBrush(color), rectangle);
        }

        public void EllipseBorderDrawing(Graphics graphics, Color borderColor)
        {
            Point newPoint = new Point(point.X - 5, point.Y - 5);
            Size size = new Size(10, 10);
            Rectangle rectangle = new Rectangle(newPoint, size);
            graphics.DrawEllipse(new Pen(borderColor, 3.0f), rectangle);
        }

        public void DayWithVisit()
        {
            this.supply = this.period;
            this.color = Color.Green;
        }

        public void DayWithoutVisit()
        {
            this.supply -= 1;
            if ((100 * supply / period) > 50)
            {
                this.color = Color.Green;
            }
            else
            {
                if ((100 * supply / period) > 0)
                {
                    this.color = Color.Yellow;
                }
                else
                {
                    this.color = Color.Red;
                }
            }
        }

        public void WriteFromVertexToRichTextBox(RichTextBox richTextBox)
        {//вывод информации о вершине в richTextBox (или ещё куда-нибудь)
            richTextBox.Text += ("\nНомер вершины: " + this.vertexID.ToString() + ";\n");
            richTextBox.Text += ("Координаты вершины: X: " + this.point.X.ToString() + ", Y: " + this.point.Y.ToString() + ";\n");
            richTextBox.Text += ("'МОП' (срок) вершины: " + this.period.ToString() + ";\n");
            richTextBox.Text += ("Запас вершины: " + this.supply.ToString() + ";\n");
            richTextBox.Text += ("Продолжительность посещения вершины: " + this.duration.ToString() + ";\n");
            richTextBox.Text += ("Время последнего посещения вершины: " + this.visiting.ToString() + ".\n");
        }

        //конструкторы
        public Vertex(int vertexID)
        {
            this.vertexID = vertexID;
            this.point.X = 0;
            this.point.Y = 0;
            this.duration = 10;
            this.period = 5;
            this.supply = 0;
            this.visiting = 0;
            if (vertexID == 0)
            {
                this.color = Color.White;
            }
            else
            {
                this.color = Color.Red;
            }
        }

        public Vertex(int vertexID, int X, int Y)
        {
            this.vertexID = vertexID;
            this.point.X = X;
            this.point.Y = Y;
            this.duration = 10;
            this.period = 5;
            this.supply = 0;
            this.visiting = 0;
            if (vertexID == 0)
            {
                this.color = Color.White;
            }
            else
            {
                this.color = Color.Red;
            }
        }

        public Vertex(int vertexID, int X, int Y, int duration, int period)
        {
            this.vertexID = vertexID;
            this.point.X = X;
            this.point.Y = Y;
            this.duration = duration;
            this.period = period;
            this.supply = 0;
            this.visiting = 0;
            if (vertexID == 0)
            {
                this.color = Color.White;
            }
            else
            {
                this.color = Color.Red;
            }
        }

        public Vertex(int vertexID, int X, int Y, int duration, int period, int supply)
        {
            this.vertexID = vertexID;
            this.point.X = X;
            this.point.Y = Y;
            this.duration = duration;
            this.period = period;
            this.supply = supply;
            this.visiting = 0;
            if (vertexID == 0)
            {
                this.color = Color.White;
            }
            else
            {
                if ((100 * supply / period) > 50)
                {
                    this.color = Color.Green;
                }
                else
                {
                    if ((100 * supply / period) > 0)
                    {
                        this.color = Color.Yellow;
                    }
                    else
                    {
                        this.color = Color.Red;
                    }
                }
            }
        }
    }

    //класс "Дуга"
    [Serializable]
    public class Edge
    {
        //свойства
        public int edgeID;//уникальный номер дуги
        public int vertexAID;//уникальный номер начальной вершины
        public int vertexBID;//уникальный номер конечной вершины
        public Point pointA;//координаты начальной вершины
        public Point pointB;//координаты конечной вершины
        public int price;//цена (вес) дуги

        //методы
        public void EdgeDrawing(Graphics graphics, Color color)
        {
            graphics.DrawLine(new Pen(color), pointA, pointB);
        }

        public void EdgeInitialization()
        {
        }

        //конструкторы
        public Edge(int edgeID)
        {
            this.edgeID = edgeID;
            this.vertexAID = 0;
            this.vertexBID = 0;
            this.pointA.X = 0;
            this.pointA.Y = 0;
            this.pointB.X = 0;
            this.pointB.Y = 0;
            this.price = 0;
        }

        public Edge(int edgeID, int vertexAID, int vertexBID, Point pointA, Point pointB, int price)
        {
            this.edgeID = edgeID;
            this.vertexAID = vertexAID;
            this.vertexBID = vertexBID;
            this.pointA.X = pointA.X;
            this.pointA.Y = pointA.Y;
            this.pointB.X = pointB.X;
            this.pointB.Y = pointB.Y;
            this.price = price;
        }
    }

    //класс "Расписание на день"
    public class TimeTable
    {
        //свойства
        public int timeTableID;
        public List<Vertex> vertices;
        public int totalTime;

        //методы
        public void Method1()
        {//вывод расписания на день в richTextBox
        }

        public void Method2()
        {//расчёт времени только на обслуживание вершин
        }

        public void Method3()
        {//расчёт времени только на прохождение по дугам
        }

        //конструкторы
        public TimeTable(int timeTableID)
        {
            this.timeTableID = timeTableID;
            this.vertices = new List<Vertex>() { };
            this.totalTime = 0;
        }
    }

    //класс "Граф"
    [Serializable]
    public class Graph
    {
        //свойста
        public int verticesNumber;
        public int edgesNumber;
        public List<Vertex> vertices { get; set; }
        public List<Edge> edges { get; set; }

        //конструкторы
        public Graph() { }
    }

    //класс "SerializedData"
    public class Serializer
    {
        //методы
        public void ObjectSerialize(string fileName, Graph graph)
        {//сериализация объекта
            FileStream fileStream = File.Open(fileName, FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, graph);
            fileStream.Close();
        }

        public Graph ObjectDeserialize(string fileName)
        {//десериализация объекта
            Graph graph = null;
            FileStream fileStream = File.Open(fileName, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            graph = (Graph)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return graph;
        }

        //конструкторы
        public Serializer() { }
    }
}

//проблема простого выбора вершин (без создания дуги)
//проблема считывания данных из файла .xml, конвертированного из файла .xls