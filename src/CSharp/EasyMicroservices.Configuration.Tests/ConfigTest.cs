using EasyMicroservices.Configuration.Interfaces;
using EasyMicroservices.Configuration.JsonConfig.Providers;
using EasyMicroservices.Configuration.Models;
using EasyMicroservices.FileManager.Interfaces;
using EasyMicroservices.FileManager.Providers.DirectoryProviders;
using EasyMicroservices.FileManager.Providers.FileProviders;
using EasyMicroservices.FileManager.Providers.PathProviders;
using EasyMicroservices.Serialization.Interfaces;
using EasyMicroservices.Serialization.Newtonsoft.Json.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EasyMicroservices.Configuration.Tests
{
    public class ConfigTest : TestBase
    {
        public ConfigTest()
        {

        }

        IFileManagerProvider GetFileProvider()
        {
            return new DiskFileProvider(new DiskDirectoryProvider(AppDomain.CurrentDomain.BaseDirectory));
        }

        ITextSerialization GetSerializer()
        {
            return new NewtonsoftJsonProvider();
        }

        void AssertFile(JsonConfigProvider loadedConfiguration, ConfigBase config, ConfigBase loaded)
        {
            Assert.True(loaded != null);
            Assert.Equal(config.ConnectionString, loaded.ConnectionString);
            Assert.Equal(config.Port, loaded.Port);
            Assert.True(config.Ports.SequenceEqual(loaded.Ports));
            Assert.Equal(config.Persons.Count, loaded.Persons.Count);
            Assert.Equal(config.LogType, loaded.LogType);
            RemoveFile(loadedConfiguration.Option.LoadedFilePath);
        }

        IConfigProvider _provider;
        [Fact]
        public async Task LoadConfigFile_WithDefaultPath_MustLoadCompletely()
        {
            var config = await GenerateConfigFile<ConfigBase>(ExactConfigFile);
            var loadedConfiguration = new JsonConfigProvider(new Option(new SystemPathProvider(), ExactConfigFile), GetFileProvider(), GetSerializer());
            var loaded = await loadedConfiguration.GetValue<ConfigBase>();
            AssertFile(loadedConfiguration, config, loaded);
        }

        [Fact]
        public async Task LoadConfigFile_WithDefaultinItilize_MustLoadCompletely()
        {
            var config = await GenerateConfigFile<ConfigBase>("Config.json");
            var loadedConfiguration = new JsonConfigProvider(new Option(new SystemPathProvider(), "Config.json"), GetFileProvider(), GetSerializer());
            var loaded = await loadedConfiguration.GetValue<ConfigBase>();
            AssertFile(loadedConfiguration, config, loaded);
        }


        [Fact]
        public async Task LoadConfigFile_WithNullOption_MustLoadCompletely()
        {

            var config = await GenerateConfigFile<ConfigBase>("Config.json");
            var loadedConfiguration = new JsonConfigProvider(GetFileProvider(), GetSerializer());
            var loaded = await loadedConfiguration.GetValue<ConfigBase>();
            AssertFile(loadedConfiguration, config, loaded);
        }

        [Fact]
        public async Task LoadConfigFile_WithMoreProperty_MustCatchException()
        {
            var config = await GenerateConfigFile<ConfigBase>(MoreConfigFile);
            var loadedConfiguration = new JsonConfigProvider(new Option(new SystemPathProvider(), MoreConfigFile), GetFileProvider(), GetSerializer());
            var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await loadedConfiguration.GetValue<ConfigBase>());
            RemoveFile(loadedConfiguration.Option.LoadedFilePath);
        }
        [Fact]
        public async Task LoadConfigFile_WithLessProperty_MustCatchException()
        {
            var config = await GenerateConfigFile<ConfigBase>(LessConfigFile);
            var loadedConfiguration = new JsonConfigProvider(new Option(new SystemPathProvider(), LessConfigFile), GetFileProvider(), GetSerializer());
            var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await loadedConfiguration.GetValue<ConfigBase>());
            RemoveFile(loadedConfiguration.Option.LoadedFilePath);

        }
        [Fact]
        public async Task LoadConfigFile_InvalidConfigFile_MustCatchException()
        {
            await GenerateInvalidConfigFile(InvalidConfigFile);
            var loadedConfiguration = new JsonConfigProvider(new Option(new SystemPathProvider(), InvalidConfigFile), GetFileProvider(), GetSerializer());
            var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await loadedConfiguration.GetValue<ConfigBase>());
            RemoveFile(loadedConfiguration.Option.LoadedFilePath);

        }
        [Fact]
        public async Task LoadConfigFile_EmptyConfigFile_MustCatchException()
        {
            await GenerateInvalidConfigFile(EmptyConfigFile);
            var loadedConfiguration = new JsonConfigProvider(new Option(new SystemPathProvider(), EmptyConfigFile), GetFileProvider(), GetSerializer());
            var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await loadedConfiguration.GetValue<ConfigBase>());
            RemoveFile(loadedConfiguration.Option.LoadedFilePath);

        }

        [Fact]
        public async Task LoadConfigFile_When_NoFileExist_MustCatchException()
        {
            var loadedConfiguration = new JsonConfigProvider(new Option(new SystemPathProvider(), "NoExistFile.json"), GetFileProvider(), GetSerializer());
            var ex = await Assert.ThrowsAnyAsync<Exception>(async () => await loadedConfiguration.GetValue<ConfigBase>());
        }
    }
}