using Amazon.S3;
using Amazon.S3.Model;

namespace MvcCoreAWSS3.Services
{
    public class ServiceStorageS3
    {
        //VAMOS A RECIBIR EL NOMBRE DEL BUCKET A PARTIR DE APPSETTINGS
        private string BucketName;
        //LA CLASE/INTERFACE PARA LOS BUCKETS SE LLAMA IAmazonS3 
        //Y TAMBIEN LA VAMOS A RECIBIR MEDIANTE INYECCION
        private IAmazonS3 ClientS3;

        public ServiceStorageS3(IConfiguration config, IAmazonS3 clientS3)
        {
            this.ClientS3 = clientS3;
            this.BucketName = config.GetValue<string>("AWS:BucketName");
        }

        //COMENZAMOS SUBIENDO UN FICHERO AL BUCKET NECESITAMOS EL nombre y Stream
        public async Task<bool> UploadFileAsync(string fileName, Stream stream)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = this.BucketName,
                Key = fileName,
                InputStream = stream
            };

            //PARA EJECUTARLO DEBEMOS HACER UNA PETICION AL CLIENT S3
            //Y NOS DEVUELVE UN RESPONSE DEL MISMO TIPO DEL REQUEST
            PutObjectResponse response = await this.ClientS3.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //METODO PARA ELIMINAR FICHERO DEL BUCKET
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            //PODEMOS TAMBIEN HACER PETICIONES SIN NECESIDAD DE REQUEST
            DeleteObjectResponse response = await this.ClientS3.DeleteObjectAsync(this.BucketName, fileName);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //PARA RECUPERAR TODOS LOS FICHEROS (URL) SE REALIZA MEDIANTE
        //VERSIONES AUNQUE NO TENGAMOS HABILITADO EL CONTROL DE VERSIONES
        //LAS KEYS SIEMPRE VAN POR VERSION
        public async Task<List<string>> GetVersionsFileAsync()
        {
            //PRIMERO RECUPERAMOS UNA RESPUESTA CON TODAS LAS VERSIONES A PARTIR DE UN BUCKET
            ListVersionsResponse response = await this.ClientS3.ListVersionsAsync(this.BucketName);
            //EXTRAEMOS TODAS LA KEYS DE NUESTROS FICHEROS
            List<string> keyFiles = response.Versions.Select(x => x.Key).ToList();
            return keyFiles;
        }

        //METODO PARA RECUPERAR FICHERO MEDIANTE SU KEY (FILENAME)
        public async Task<Stream> GetPrivateFileAsync(string fileName)
        {
            GetObjectResponse response = await this.ClientS3.GetObjectAsync(this.BucketName, fileName);
            return response.ResponseStream;
        }
    }
}
