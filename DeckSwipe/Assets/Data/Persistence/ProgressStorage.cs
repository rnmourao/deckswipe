using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Persistence {
	
	public class ProgressStorage {
		
		public GameProgress Progress { get; private set; }
		public Task ProgressLoadTask { get; }
		
		private static readonly string gameProgressPath = Application.persistentDataPath + "/progress.json";
		
		public ProgressStorage(CardStorage cardStorage) {
			ProgressLoadTask = Load(cardStorage);
		}
		
		public async Task Load(CardStorage cardStorage) {
			Progress = await LoadLocally();
			if (Progress != null) {
				Progress.AttachReferences(cardStorage);
			}
			else {
				Progress = new GameProgress(cardStorage);
			}
		}
		
		public async Task<GameProgress> LoadLocally() {
			if (File.Exists(gameProgressPath)) {
				string progressJson;
				using (FileStream fileStream = File.OpenRead(gameProgressPath)) {
					StreamReader reader = new StreamReader(fileStream);
					progressJson = await reader.ReadToEndAsync();
				}
				return JsonUtility.FromJson<GameProgress>(progressJson);
			}
			else {
				return null;
			}
		}
		
		public async void SaveLocally() {
			string progressJson = JsonUtility.ToJson(Progress);
			using (FileStream fileStream = File.Create(gameProgressPath)) {
				StreamWriter writer = new StreamWriter(fileStream);
				await writer.WriteAsync(progressJson);
				await writer.WriteAsync('\n');
				await writer.FlushAsync();
			}
		}
		
	}
	
}
