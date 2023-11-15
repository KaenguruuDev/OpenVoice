using System.Reflection;

namespace OpenVoice
{
    // Signal Class for internal handling
    public class Signal
    {
        // Callable Class [INTERNAL]
        private class Callable
        {
            private MethodInfo Method;
            private Object[] Parameters;
            private Object Obj;
            private int Id;
            public Callable(MethodInfo Method, Object[] Parameters, Object Obj)
            {
                this.Method = Method;
                this.Parameters = Parameters;
                this.Obj = Obj;
                Id = GetHashCode();
            }

            public MethodInfo GetMethod()
            { return Method; }
            public Object[] GetParamters()
            { return Parameters; }
            public Object GetObj()
            { return Obj; }
            public int GetId()
            { return Id; }
        }

        // Attributes
        private string Name;
        private int Id = 0;
        private List<Callable> Callables;

        // Constructor
        public Signal(string Name)
        {
            if (SignalHandler.GetSignal(Name) != null) throw new InvalidDataException("A Signal with the name" + Name + " already exists!");
            this.Name = Name;
            Id = GetHashCode();

            Callables = new List<Callable>();
        }

        // Adds Callable to list.
        public int AddCallable(MethodInfo Function, Object[] Parameters, Object? Obj)
        {
            Callable? NewCallable = new Callable(Function, Parameters, Obj);
            Callables.Add(NewCallable);
            return NewCallable.GetId();
        }

        // Removes Callable from list. Returns true if successful, else false.
        public bool RemoveCallable(int Id)
        {
            foreach (Callable Func in Callables)
            {
                if (Func.GetId() == Id) return Callables.Remove(Func);
            }
            return false;
        }

        public void Emit()
        {
            foreach (Callable Function in Callables)
            { Function.GetMethod().Invoke(Function.GetObj(), Function.GetParamters()); }
        }

        public string GetName() { return Name; }
        public int GetId() { return Id; }
    }

    // [HELPER CLASS]
    public class SignalInfo
    {
        private Signal? Signal;
        private Object? Object;
        private string Method;
        private Object[]? Params;
        private bool EmitOnConnect;
        
        public SignalInfo(Signal Signal, Object Object, string Method, Object[] Params, bool EmitOnConnect)
        {
            this.Signal = Signal;
            this.Object = Object;
            this.Method = Method;
            this.Params = Params;
            this.EmitOnConnect = EmitOnConnect;
        }

        public Signal? GetSignal()
        { return Signal; }
        public Object? GetObject()
        { return Object; }
        public string? GetMethod()
        { return Method; }
        public Object[]? GetParams()
        { return Params; }
        public bool GetEmitOnConnect()
        { return EmitOnConnect; }
    }

    public class SignalHandler
    {
        static List<Signal>? Signals;

        static SignalHandler()
        {
            Signals = new List<Signal>();
        }

        public static int ConnectSignalToObj(Signal? S, Object? Obj, string? Function, Object[]? Parameters)
        {
            if (S == null || Obj == null || Function == null || Parameters == null) return int.MinValue;
            Type ObjType = Obj.GetType();
            MethodInfo? Callable = ObjType.GetMethod(Function, BindingFlags.Instance | BindingFlags.NonPublic);
            if (Callable == null) return int.MinValue;

            return S.AddCallable(Callable, Parameters, Obj);
            
        }

        // Disconnects Signal S from Object Obj
        public static void DisconnectSignalFromObj(Signal S, int Id)
        {
            if (S == null) return;
            S.RemoveCallable(Id);
        }

        
        public static void EmitSignal(Signal S)
        {
            if (S == null) return;
            S.Emit();
        }


        // Get Signal based on Name. Returns null if not found
        public static Signal? GetSignal(string SignalName)
        {
            if (Signals == null) return null;
            for (int i = 0; i < Signals.Count; i++)
            {
                if (Signals[i].GetName() == SignalName) return Signals[i];
            }
            return null;
        }
        // Get Signal based on Id. Returns null if not found
        public static Signal? GetSignal(int Id)
        {
            if (Signals == null) return null;
            for (int i = 0; i < Signals.Count; i++)
            {
                if (Signals[i].GetId() == Id) return Signals[i];
            }
            return null;
        }
    }
}
