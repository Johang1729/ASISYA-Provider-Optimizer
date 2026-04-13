import React, { useState, useEffect } from 'react';
import { ShieldCheck, Truck, Clock, Navigation, CheckCircle, Activity, LogOut, ArrowRight, Zap, Target } from 'lucide-react';

const MOCK_PROVIDERS = [
  { id: 1, name: "Grúas Elite Premium", status: "Libre", eta: 12, rating: 4.8, type: "Plataforma Pesada" },
  { id: 2, name: "Asistencia Ruta 66", status: "Libre", eta: 25, rating: 4.5, type: "Plataforma Pesada" },
  { id: 3, name: "Mecánica Automotriz Juan", status: "Ocupado", eta: 40, rating: 4.2, type: "Remolque Básico" },
  { id: 4, name: "Cerrajería Express 24/7", status: "Libre", eta: 8, rating: 4.9, type: "Asistencia Rápida" },
  { id: 5, name: "Rescate Vehicular SV", status: "Libre", eta: 18, rating: 4.6, type: "Plataforma Plana" },
  { id: 6, name: "Grúas Mega (Base Sur)", status: "Libre", eta: 15, rating: 4.1, type: "Plataforma Pesada" }
];

export default function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isConsulting, setIsConsulting] = useState(false);
  const [providers, setProviders] = useState([]);

  useEffect(() => {
    // Check local storage for active session token
    const token = localStorage.getItem('empresa_jwt');
    if (token) setIsAuthenticated(true);
  }, []);

  const handleLogin = () => {
    // Set authentication token
    localStorage.setItem('empresa_jwt', 'jwt_777_access_granted');
    setIsAuthenticated(true);
  };

  const handleLogout = () => {
    // Clear session token and reset state
    localStorage.removeItem('empresa_jwt');
    setIsAuthenticated(false);
    setProviders([]);
  };

  const consultProviders = () => {
    setIsConsulting(true);
    setProviders([]);
    
    // Fetch provider matrix
    setTimeout(() => {
      // Sort and filter active units route-distance prioritization
      const sorted = [...MOCK_PROVIDERS]
        .filter(p => p.status === "Libre")
        .sort((a, b) => a.eta - b.eta);
      
      setProviders(sorted);
      setIsConsulting(false);
    }, 1800);
  };

  // --- LOGIN VIEW ---
  if (!isAuthenticated) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-950 relative overflow-hidden font-sans">
        
        {/* Decorative background */}
        <div className="absolute inset-0 overflow-hidden pointer-events-none">
          <div className="absolute top-[-20%] left-[-10%] w-[50vw] h-[50vw] rounded-full bg-violet-600/20 blur-[120px] animate-float"></div>
          <div className="absolute bottom-[-20%] right-[-10%] w-[50vw] h-[50vw] rounded-full bg-cyan-600/20 blur-[120px] animate-float" style={{ animationDelay: '3s' }}></div>
        </div>

        <div className="glass-panel p-12 rounded-3xl w-full max-w-lg text-center relative z-10 transition-all hover:shadow-[0_0_80px_rgba(139,92,246,0.15)]">
          <div className="flex justify-center mb-8">
            <div className="bg-gradient-to-br from-violet-500 to-cyan-400 p-1 rounded-2xl shadow-2xl shadow-violet-500/25">
               <div className="bg-slate-900 p-5 rounded-xl">
                 <ShieldCheck size={56} className="text-cyan-400" strokeWidth={1.5} />
               </div>
            </div>
          </div>
          
          <h1 className="text-4xl font-extrabold text-transparent bg-clip-text bg-gradient-to-r from-white to-slate-400 mb-3 tracking-tight">
            Empresa
          </h1>
          <p className="text-cyan-400 font-medium mb-10 tracking-widest text-sm uppercase letter-spacing-2">
            Provider Optimizer Terminal
          </p>
          
          <button 
            onClick={handleLogin}
            className="group relative w-full bg-gradient-to-r from-violet-600 to-indigo-600 hover:from-violet-500 hover:to-indigo-500 text-white font-semibold py-4 flex items-center justify-center gap-3 rounded-xl transition-all shadow-[0_0_30px_rgba(139,92,246,0.3)] hover:shadow-[0_0_40px_rgba(139,92,246,0.5)] overflow-hidden"
          >
            <div className="absolute inset-0 bg-white/20 translate-y-full group-hover:translate-y-0 transition-transform duration-300 ease-in-out"></div>
            <span className="relative z-10 flex items-center gap-2">Validar Identidad JWT <ArrowRight size={20} className="group-hover:translate-x-1 transition-transform"/></span>
          </button>
          
          <div className="mt-10 pt-6 border-t border-slate-700/50 flex justify-center gap-3 items-center">
             <span className="relative flex h-3 w-3">
               <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-cyan-400 opacity-75"></span>
               <span className="relative inline-flex rounded-full h-3 w-3 bg-cyan-500"></span>
             </span>
             <span className="text-xs font-semibold text-slate-400 tracking-wider">SECURE GATEWAY ENCRYPTED</span>
          </div>
        </div>
      </div>
    );
  }

  // --- MAIN DASHBOARD VIEW ---
  return (
    <div className="min-h-screen bg-slate-950 font-sans selection:bg-violet-500/30 text-slate-200 relative overflow-hidden">
      
      {/* Global styling */}
      <div className="fixed inset-0 pointer-events-none">
        <div className="absolute top-0 right-0 w-[60vw] h-[60vw] bg-indigo-900/10 blur-[150px] rounded-full mix-blend-screen"></div>
        <div className="absolute bottom-0 left-0 w-[50vw] h-[50vw] bg-violet-900/10 blur-[130px] rounded-full mix-blend-screen"></div>
      </div>

      {/* Navigation Header */}
      <header className="glass-panel border-b border-white/5 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-20 flex items-center justify-between">
          <div className="flex items-center gap-4">
            <div className="bg-gradient-to-tr from-cyan-500 to-blue-500 p-2 rounded-xl shadow-lg shadow-cyan-500/20">
              <Activity size={24} className="text-slate-950" strokeWidth={2} />
            </div>
            <h1 className="font-bold text-2xl tracking-wide flex items-center gap-3">
              <span className="text-white">Empresa</span>
              <span className="w-1 h-6 bg-slate-700 rounded-full"></span> 
              <span className="text-transparent bg-clip-text bg-gradient-to-r from-cyan-400 to-violet-400">Optimization Engine</span>
            </h1>
          </div>
          <div className="flex items-center gap-6">
            <div className="hidden sm:flex items-center gap-2 text-xs font-medium glass-panel px-4 py-2 rounded-full border border-white/5">
              <span className="relative flex h-2 w-2">
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-cyan-400 opacity-75"></span>
                <span className="relative inline-flex rounded-full h-2 w-2 bg-cyan-500"></span>
              </span>
              <span className="text-cyan-200">System Online</span>
            </div>
            <button onClick={handleLogout} className="text-slate-400 hover:text-white transition-colors bg-white/5 p-2.5 rounded-full hover:bg-white/10 hover:shadow-[0_0_15px_rgba(255,255,255,0.1)]">
              <LogOut size={20} />
            </button>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 relative z-10 animate-in slide-in-from-bottom-8 duration-700 ease-out">
        
        {/* Ticket Metadata Section */}
        <div className="glass-panel rounded-3xl p-8 mb-10 flex flex-col md:flex-row justify-between items-center gap-6 border-white/10 relative overflow-hidden">
          <div className="absolute top-0 left-0 w-full h-[1px] bg-gradient-to-r from-transparent via-cyan-500/50 to-transparent"></div>
          
          <div className="flex items-start gap-5">
            <div className="bg-rose-500/10 p-4 rounded-2xl text-rose-400 border border-rose-500/20 shadow-[0_0_30px_rgba(244,63,94,0.15)]">
              <Target size={28} />
            </div>
            <div>
              <h2 className="text-2xl font-bold text-white mb-2 tracking-tight">Ticket de Siniestro #8892-X</h2>
              <div className="flex flex-wrap gap-3 text-sm font-medium">
                <span className="bg-slate-800/80 text-cyan-300 px-3 py-1.5 rounded-lg border border-cyan-500/20">Tipo: Colisión Fuerte</span>
                <span className="bg-slate-800/80 text-violet-300 px-3 py-1.5 rounded-lg border border-violet-500/20">Clase: Vehículo Pesado</span>
                <span className="bg-slate-800/80 text-slate-300 px-3 py-1.5 rounded-lg border border-slate-600/50">Location: Autopista Norte</span>
              </div>
            </div>
          </div>
          
          <button 
            onClick={consultProviders}
            disabled={isConsulting}
            className="w-full md:w-auto relative group bg-gradient-to-r from-cyan-600 to-blue-600 hover:from-cyan-500 hover:to-blue-500 disabled:from-slate-700 disabled:to-slate-800 text-white font-bold px-8 py-4 rounded-2xl flex items-center justify-center gap-3 transition-all shadow-[0_0_40px_rgba(6,182,212,0.3)] hover:shadow-[0_0_60px_rgba(6,182,212,0.5)] border border-cyan-400/30 overflow-hidden"
          >
            <div className="absolute inset-0 bg-white/10 translate-x-[-100%] group-hover:translate-x-0 transition-transform duration-500 ease-in-out"></div>
            {isConsulting ? (
              <><Activity className="animate-spin relative z-10" size={22}/> <span className="relative z-10 tracking-wide">Calculando...</span></>
            ) : (
              <><Zap size={22} className="relative z-10 text-cyan-200 fill-cyan-400/50"/> <span className="relative z-10 tracking-wide">Lanzar</span></>
            )}
          </button>
        </div>

        {/* Providers Data Grid */}
        <div className="glass-panel rounded-3xl overflow-hidden border-white/10 shadow-2xl">
          <div className="px-8 py-6 border-b border-white/5 bg-slate-900/50 flex flex-col sm:flex-row justify-between items-center gap-4">
            <h3 className="font-bold text-white text-xl flex items-center gap-3">
              <span className="w-2 h-8 bg-gradient-to-b from-cyan-400 to-blue-600 rounded-full"></span>
              Live Fallback Board
            </h3>
            
            {providers.length > 0 && (
              <span className="text-xs font-extrabold bg-violet-500/20 text-violet-300 px-4 py-2 rounded-xl border border-violet-500/30 uppercase tracking-widest shadow-[0_0_20px_rgba(139,92,246,0.2)]">
                {providers.length} Contratistas en Zona
              </span>
            )}
          </div>
          
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse whitespace-nowrap">
              <thead>
                <tr className="bg-slate-900/80 text-slate-400 text-xs font-bold uppercase tracking-widest border-b border-white/5">
                  <th className="px-8 py-6">Red Operativa</th>
                  <th className="px-8 py-6 text-center">Status Global</th>
                  <th className="px-8 py-6 text-center">Precisión ETA</th>
                  <th className="px-8 py-6 text-center">Trust Rating</th>
                  <th className="px-8 py-6 text-right">Acción</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-white/5">
                
                {/* Empty / Initial State */}
                {providers.length === 0 && !isConsulting && (
                  <tr>
                    <td colSpan="5" className="px-8 py-32 text-center">
                      <div className="inline-flex justify-center items-center w-24 h-24 bg-slate-800/50 rounded-full mb-6 border border-white/5 shadow-inner relative">
                        <div className="absolute inset-0 rounded-full border border-cyan-500/20 animate-ping"></div>
                        <Navigation className="text-slate-500" size={40} />
                      </div>
                      <h4 className="text-2xl font-bold text-slate-300 mb-2">Escáner en Reposo</h4>
                      <p className="text-slate-500 max-w-md mx-auto text-lg leading-relaxed">
                        Inicie el motor de optimización.
                      </p>
                    </td>
                  </tr>
                )}
                
                {/* Active Providers Render */}
                {providers.map((p, index) => {
                  
                  // Selected Priority Unit
                  const isWinner = index === 0;

                  return (
                    <tr key={p.id} className={`group relative transition-all duration-300 hover:bg-white/5 ${isWinner ? 'bg-gradient-to-r from-blue-900/30 to-transparent' : ''}`}>
                      
                      <td className="px-8 py-6">
                        <div className="flex items-center gap-5 relative z-10">
                          <div className={`p-4 rounded-2xl shadow-lg border relative ${isWinner ? 'bg-gradient-to-br from-cyan-500 to-blue-600 text-white border-cyan-400 shadow-cyan-500/30' : 'bg-slate-800 text-slate-400 border-white/5 group-hover:border-white/20'}`}>
                            {isWinner && <div className="absolute -top-1 -right-1 w-3 h-3 bg-white rounded-full animate-ping"></div>}
                            <Truck size={24} />
                          </div>
                          <div>
                            <div className="flex items-center gap-3 mb-1">
                              <p className={`font-bold text-lg tracking-tight ${isWinner ? 'text-white' : 'text-slate-200'}`}>
                                {p.name} 
                              </p>
                              {isWinner && (
                                <span className="text-[10px] bg-gradient-to-r from-amber-500 to-orange-500 text-white px-3 py-1 rounded-full uppercase tracking-widest font-black shadow-[0_0_15px_rgba(245,158,11,0.5)] border border-amber-300/50 flex items-center gap-1.5">
                                  <Zap size={12} className="fill-white"/> PING DIRECTO
                                </span>
                              )}
                            </div>
                            <p className="text-sm font-medium text-slate-500">{p.type}</p>
                          </div>
                        </div>
                      </td>
                      
                      <td className="px-8 py-6 text-center">
                        <span className="inline-flex items-center justify-center gap-2 px-3.5 py-1.5 rounded-xl text-xs font-bold bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 backdrop-blur-md">
                          <span className="w-2 h-2 rounded-full bg-emerald-400 animate-pulse shadow-[0_0_10px_rgba(52,211,153,0.8)]"></span>
                          Ready for Route
                        </span>
                      </td>
                      
                      <td className="px-8 py-6 text-center">
                        <div className="flex items-center justify-center gap-2.5">
                          <Clock size={18} className={p.eta <= 15 ? 'text-cyan-400' : 'text-violet-400'} />
                          <span className={`font-black text-2xl tracking-tighter ${p.eta < 15 ? 'text-white' : 'text-slate-300'}`}>
                            {p.eta} <span className="text-xs font-semibold uppercase tracking-widest text-slate-500 ml-1">Min</span>
                          </span>
                        </div>
                      </td>
                      
                      <td className="px-8 py-6 text-center">
                        <div className="inline-flex items-center gap-1.5 bg-slate-800/80 px-3 py-1.5 rounded-lg border border-slate-700 shadow-inner">
                          <span className="text-violet-400 text-sm">★</span>
                          <span className="text-sm font-bold text-slate-200">{p.rating.toFixed(1)}</span>
                        </div>
                      </td>
                      
                      <td className="px-8 py-6 text-right">
                        {isWinner ? (
                          <button className="relative text-sm font-bold text-white bg-white/10 hover:bg-white/20 border border-white/20 px-6 py-3 rounded-xl transition-all shadow-[0_0_20px_rgba(255,255,255,0.05)] overflow-hidden">
                            <div className="absolute top-0 left-0 h-full w-[20%] bg-white/20 transform skew-x-12 animate-[slide_2s_infinite]"></div>
                            Aguardando (0:44s)
                          </button>
                        ) : (
                          <button className="text-sm font-semibold text-slate-400 hover:text-white border border-slate-700 hover:border-slate-500 bg-transparent hover:bg-slate-800 px-6 py-3 rounded-xl transition-all">
                            Details
                          </button>
                        )}
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        </div>
      </main>
    </div>
  );
}
