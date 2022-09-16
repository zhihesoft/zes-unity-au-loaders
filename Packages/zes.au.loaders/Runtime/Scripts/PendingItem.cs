using System.Threading.Tasks;

namespace Au.Loaders
{
    public class PendingItem
    {
        public bool completed { get; private set; } = false;
        public bool succ { get; private set; } = false;

        private object data;

        public void SetData(object data)
        {
            completed = true;
            succ = true;
            this.data = data;
        }

        public void SetFailed()
        {
            completed = true;
            succ = false;
            data = null;
        }

        public async Task Wait()
        {
            await Utils.WaitUntil(() => completed);
        }

        public T GetData<T>()
        {
            return (T)data;
        }
    }
}
