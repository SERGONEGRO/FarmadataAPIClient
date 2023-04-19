using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace FarmadataAPIClient
{
    class Program
    {
        const string apiUrl = "http://f3bus.test.pharmadata.ru";
        const string path = @"C:\temp\goods.txt";   // путь к файлу

        static async Task Main(string[] args)
        {
            // Запрос токена
            string accessToken = GetToken(apiUrl + "/User/auth/Agent");
            Console.WriteLine("Токен доступа получен."); //: " + accessToken);

            //Запрос списка департаментов
            var departments = GetDepartments(apiUrl + "/User/departments", accessToken);
            Console.WriteLine("Департаментов всего: " + departments.Count);

            //Выбираем рандомно департамент из списка
            Root currentDepartment = GetRandomDepartment(departments);

            // Запрос номенклатуры для данного департмента
            List<Good> goods = GetGoodsFromDepartment(currentDepartment, accessToken);
            Console.WriteLine("Номенклатуры всего: " + goods.Count);

            //запись в файл
            WriteToFile(currentDepartment, goods);
            Console.WriteLine("Файл записан: " + path);

        }

        /// <summary>
        /// запись в файл
        /// </summary>
        /// <param name="currentDepartment"></param>
        /// <param name="goods"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static async void WriteToFile(Root currentDepartment, List<Good> goods)
        {
            string text = "**** Департамент: " + currentDepartment.department.name + " ****\n"
                        + "**** Список товаров: ****\n";

            // полная перезапись файла 
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(text);
                for (int i = 1; i < goods.Count; i++)
                {
                    writer.WriteLine("" + i + ". Наименование: " + goods[i].name + ", Производитель: " + goods[i].producer);
                }
            }
        }

        /// <summary>
        /// получаем список товаров
        /// </summary>
        /// <param name="currentDepartment"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static List<Good> GetGoodsFromDepartment(Root currentDepartment, string accessToken)
        {
            //получаем id департамента
            string id = currentDepartment.department.id;
            HttpWebRequest goodsRequest = (HttpWebRequest)HttpWebRequest.Create(apiUrl + "/Goods/" + id);
            goodsRequest.Method = "GET";
            goodsRequest.Headers.Add("Authorization", "Bearer" + " " + accessToken);

            HttpWebResponse goodsResponse = (HttpWebResponse)goodsRequest.GetResponse();

            // Чтение ответа товаров
            string goodsJson;

            using (Stream goodStream = goodsResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(goodStream, Encoding.UTF8);
                goodsJson = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<List<Good>>(goodsJson);

        }

        /// <summary>
        /// Получает токен доступа
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetToken(string url)
        {
            string postData = "{\"login\":\"demo\",\"password\":\"demo\"}";

            HttpWebRequest loginRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/json";
            loginRequest.ContentLength = postData.Length;

            using (Stream dataStream = loginRequest.GetRequestStream())
            {
                dataStream.Write(Encoding.UTF8.GetBytes(postData), 0, postData.Length);
            }

            HttpWebResponse loginResponse = (HttpWebResponse)loginRequest.GetResponse();

            // Чтение ответа авторизации
            string jsonResponse;

            using (Stream responseStream = loginResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                jsonResponse = reader.ReadToEnd();
            }

            JObject loginResult = JObject.Parse(jsonResponse);
            return (string)loginResult["token"];
        }


        /// <summary>
        /// получает список департаментов
        /// </summary>
        /// <param name="url"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static List<Root>? GetDepartments(string url, string accessToken)
        {
            HttpWebRequest depsRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            depsRequest.Method = "GET";
            depsRequest.Headers.Add("Authorization", "Bearer" + " " + accessToken);

            HttpWebResponse depsResponse = (HttpWebResponse)depsRequest.GetResponse();

            // Чтение ответа департаментов
            string depsJson;

            using (Stream depsStream = depsResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(depsStream, Encoding.UTF8);
                depsJson = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<List<Root>>(depsJson);    
        }

        /// <summary>
        /// Возвращает рандомный департамент
        /// </summary>
        /// <param name="departments"></param>
        /// <returns></returns>
        private static Root GetRandomDepartment(List<Root> departments)
        {
            Random rand = new Random();
            return departments[rand.Next(0, departments.Count)];
        }

    }
}