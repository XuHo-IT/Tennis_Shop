// =====================================================
// TENNIS SHOP - ADMIN PANEL JAVASCRIPT
// Modern, Professional & Smooth Interactions
// =====================================================

(function() {
    'use strict';

    // =====================================================
    // SIDEBAR TOGGLE
    // =====================================================
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', function() {
            if (window.innerWidth > 768) {
                sidebar.classList.toggle('collapsed');
                mainContent?.classList.toggle('expanded');
                
                const isCollapsed = sidebar.classList.contains('collapsed');
                localStorage.setItem('sidebarCollapsed', isCollapsed);
            } else {
                sidebar.classList.toggle('mobile-show');
            }
        });

        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', function(e) {
            if (window.innerWidth <= 768 && 
                sidebar.classList.contains('mobile-show') && 
                !sidebar.contains(e.target) && 
                e.target !== sidebarToggle) {
                sidebar.classList.remove('mobile-show');
            }
        });
    }

    // =====================================================
    // RESTORE SIDEBAR STATE
    // =====================================================
    window.addEventListener('DOMContentLoaded', function() {
        if (window.innerWidth > 768 && sidebar && mainContent) {
            const sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
            if (sidebarCollapsed) {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
            }
        }
    });

    // =====================================================
    // ACTIVE MENU ITEM HIGHLIGHTING
    // =====================================================
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.menu-link').forEach(link => {
        const href = link.getAttribute('href');
        if (href && currentPath.includes(href.toLowerCase())) {
            link.classList.add('active');
        }
    });

    // =====================================================
    // AUTO-HIDE ALERTS
    // =====================================================
    setTimeout(function() {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(alert => {
            if (typeof bootstrap !== 'undefined' && bootstrap.Alert) {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            } else {
                alert.style.opacity = '0';
                alert.style.transform = 'translateY(-20px)';
                setTimeout(() => alert.remove(), 300);
            }
        });
    }, 5000);

    // =====================================================
    // CONFIRM DELETE ACTIONS
    // =====================================================
    document.querySelectorAll('[data-confirm-delete]').forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
                e.preventDefault();
            }
        });
    });

    // =====================================================
    // NUMBER COUNTER ANIMATION
    // =====================================================
    function animateValue(element, start, end, duration, prefix = '', suffix = '') {
        let startTimestamp = null;
        const isDecimal = end % 1 !== 0;
        
        const step = (timestamp) => {
            if (!startTimestamp) startTimestamp = timestamp;
            const progress = Math.min((timestamp - startTimestamp) / duration, 1);
            const value = progress * (end - start) + start;
            
            if (isDecimal) {
                element.textContent = prefix + value.toFixed(2) + suffix;
            } else {
                element.textContent = prefix + Math.floor(value).toLocaleString() + suffix;
            }
            
            if (progress < 1) {
                window.requestAnimationFrame(step);
            }
        };
        window.requestAnimationFrame(step);
    }

    // Animate stat numbers on page load
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && !entry.target.classList.contains('animated')) {
                const element = entry.target;
                const text = element.textContent.trim();
                const hasPrefix = text.startsWith('$');
                const hasSuffix = text.includes('%');
                
                let value = parseFloat(text.replace(/[$,%]/g, '').replace(/,/g, ''));
                
                if (!isNaN(value)) {
                    element.classList.add('animated');
                    animateValue(
                        element, 
                        0, 
                        value, 
                        1200,
                        hasPrefix ? '$' : '',
                        hasSuffix ? '%' : ''
                    );
                }
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.stat-value, .stat-number').forEach(element => {
        observer.observe(element);
    });

    // =====================================================
    // SMOOTH SCROLL TO TOP
    // =====================================================
    const scrollTopBtn = document.createElement('button');
    scrollTopBtn.innerHTML = '<i class="fas fa-arrow-up"></i>';
    scrollTopBtn.className = 'scroll-to-top';
    scrollTopBtn.style.cssText = `
        position: fixed;
        bottom: 2rem;
        right: 2rem;
        width: 48px;
        height: 48px;
        border-radius: 50%;
        background: linear-gradient(135deg, #6366f1, #818cf8);
        color: white;
        border: none;
        cursor: pointer;
        box-shadow: 0 4px 12px rgba(99, 102, 241, 0.3);
        opacity: 0;
        visibility: hidden;
        transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        z-index: 999;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 1.25rem;
    `;
    document.body.appendChild(scrollTopBtn);

    window.addEventListener('scroll', () => {
        if (window.scrollY > 300) {
            scrollTopBtn.style.opacity = '1';
            scrollTopBtn.style.visibility = 'visible';
        } else {
            scrollTopBtn.style.opacity = '0';
            scrollTopBtn.style.visibility = 'hidden';
        }
    });

    scrollTopBtn.addEventListener('click', () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });

    scrollTopBtn.addEventListener('mouseenter', () => {
        scrollTopBtn.style.transform = 'translateY(-4px)';
        scrollTopBtn.style.boxShadow = '0 8px 20px rgba(99, 102, 241, 0.4)';
    });

    scrollTopBtn.addEventListener('mouseleave', () => {
        scrollTopBtn.style.transform = 'translateY(0)';
        scrollTopBtn.style.boxShadow = '0 4px 12px rgba(99, 102, 241, 0.3)';
    });

    // =====================================================
    // CHART.JS DEFAULT CONFIGURATION
    // =====================================================
    if (typeof Chart !== 'undefined') {
        Chart.defaults.font.family = "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif";
        Chart.defaults.color = '#6b7280';
        Chart.defaults.plugins.tooltip.backgroundColor = 'rgba(17, 24, 39, 0.95)';
        Chart.defaults.plugins.tooltip.padding = 12;
        Chart.defaults.plugins.tooltip.borderRadius = 10;
        Chart.defaults.plugins.tooltip.titleFont = { size: 14, weight: 'bold' };
        Chart.defaults.plugins.tooltip.bodyFont = { size: 13 };
        Chart.defaults.plugins.legend.labels.usePointStyle = true;
        Chart.defaults.plugins.legend.labels.padding = 15;
    }

    // =====================================================
    // LOADING OVERLAY
    // =====================================================
    window.showLoading = function() {
        let overlay = document.getElementById('loadingOverlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'loadingOverlay';
            overlay.style.cssText = `
                position: fixed;
                inset: 0;
                background: rgba(255, 255, 255, 0.9);
                backdrop-filter: blur(4px);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 9999;
                animation: fadeIn 0.2s ease;
            `;
            overlay.innerHTML = `
                <div style="text-align: center;">
                    <div style="
                        width: 60px;
                        height: 60px;
                        border: 4px solid #e5e7eb;
                        border-top-color: #6366f1;
                        border-radius: 50%;
                        animation: spin 0.8s linear infinite;
                        margin: 0 auto 1rem;
                    "></div>
                    <p style="color: #6b7280; font-weight: 600;">Loading...</p>
                </div>
            `;
            document.body.appendChild(overlay);
            
            // Add spin animation
            if (!document.getElementById('spinAnimation')) {
                const style = document.createElement('style');
                style.id = 'spinAnimation';
                style.textContent = `
                    @keyframes spin {
                        to { transform: rotate(360deg); }
                    }
                    @keyframes fadeIn {
                        from { opacity: 0; }
                        to { opacity: 1; }
                    }
                `;
                document.head.appendChild(style);
            }
        }
    };

    window.hideLoading = function() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => overlay.remove(), 300);
        }
    };

    // =====================================================
    // UTILITY FUNCTIONS
    // =====================================================
    window.getStatusBadgeClass = function(status) {
        const statusLower = (status || '').toLowerCase();
        const statusClasses = {
            'pending': 'warning',
            'processing': 'info',
            'shipped': 'primary',
            'completed': 'success',
            'cancelled': 'danger',
            'active': 'success',
            'inactive': 'secondary'
        };
        return statusClasses[statusLower] || 'secondary';
    };

    window.formatCurrency = function(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            minimumFractionDigits: 2
        }).format(amount);
    };

    window.formatDate = function(dateString) {
        const options = { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        return new Date(dateString).toLocaleDateString('en-US', options);
    };

    window.formatNumber = function(number) {
        return new Intl.NumberFormat('en-US').format(number);
    };

    // =====================================================
    // RESPONSIVE TABLE WRAPPER
    // =====================================================
    document.querySelectorAll('.admin-table, .modern-table').forEach(table => {
        if (!table.parentElement.classList.contains('table-responsive')) {
            const wrapper = document.createElement('div');
            wrapper.className = 'table-responsive';
            table.parentNode.insertBefore(wrapper, table);
            wrapper.appendChild(table);
        }
    });

    // =====================================================
    // TOAST NOTIFICATION SYSTEM
    // =====================================================
    window.showToast = function(message, type = 'info') {
        const toast = document.createElement('div');
        const bgColor = {
            'success': 'linear-gradient(135deg, #10b981, #34d399)',
            'error': 'linear-gradient(135deg, #ef4444, #f87171)',
            'warning': 'linear-gradient(135deg, #f59e0b, #fbbf24)',
            'info': 'linear-gradient(135deg, #6366f1, #818cf8)'
        }[type] || 'linear-gradient(135deg, #6366f1, #818cf8)';

        toast.style.cssText = `
            position: fixed;
            top: 2rem;
            right: 2rem;
            background: ${bgColor};
            color: white;
            padding: 1rem 1.5rem;
            border-radius: 12px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
            z-index: 9999;
            animation: slideInRight 0.3s ease;
            min-width: 300px;
            font-weight: 600;
        `;
        toast.textContent = message;
        document.body.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'slideOutRight 0.3s ease';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    };

    // Add slide animations
    if (!document.getElementById('toastAnimations')) {
        const style = document.createElement('style');
        style.id = 'toastAnimations';
        style.textContent = `
            @keyframes slideInRight {
                from {
                    opacity: 0;
                    transform: translateX(100%);
                }
                to {
                    opacity: 1;
                    transform: translateX(0);
                }
            }
            @keyframes slideOutRight {
                from {
                    opacity: 1;
                    transform: translateX(0);
                }
                to {
                    opacity: 0;
                    transform: translateX(100%);
                }
            }
        `;
        document.head.appendChild(style);
    }

    // =====================================================
    // ENHANCED TABLE INTERACTIONS
    // =====================================================
    document.querySelectorAll('.modern-table tbody tr').forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.style.transition = 'all 0.3s ease';
        });
    });

    // =====================================================
    // KEYBOARD SHORTCUTS
    // =====================================================
    document.addEventListener('keydown', function(e) {
        // Ctrl/Cmd + K for search focus
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            const searchInput = document.querySelector('[id*="Search"], .search-box input');
            if (searchInput) {
                searchInput.focus();
                showToast('Search activated', 'info');
            }
        }

        // Escape to clear search
        if (e.key === 'Escape') {
            const searchInputs = document.querySelectorAll('[id*="Search"], .search-box input');
            searchInputs.forEach(input => {
                if (input === document.activeElement) {
                    input.value = '';
                    input.blur();
                    // Trigger input event to update filters
                    input.dispatchEvent(new Event('input', { bubbles: true }));
                }
            });
        }
    });

    // =====================================================
    // PROGRESS BAR ANIMATION ON SCROLL
    // =====================================================
    const progressBars = document.querySelectorAll('.progress-fill');
    const progressObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && !entry.target.dataset.animated) {
                const width = entry.target.style.width;
                entry.target.style.width = '0';
                setTimeout(() => {
                    entry.target.style.width = width;
                    entry.target.dataset.animated = 'true';
                }, 100);
            }
        });
    }, { threshold: 0.5 });

    progressBars.forEach(bar => progressObserver.observe(bar));

    // =====================================================
    // CARD HOVER EFFECTS
    // =====================================================
    document.querySelectorAll('.card, .stat-card, .insight-card, .metric-card').forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transition = 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)';
        });
    });

    // =====================================================
    // FORM VALIDATION ENHANCEMENT
    // =====================================================
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function(e) {
            const requiredInputs = form.querySelectorAll('[required]');
            let isValid = true;

            requiredInputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.style.borderColor = '#ef4444';
                    setTimeout(() => {
                        input.style.borderColor = '';
                    }, 2000);
                }
            });

            if (!isValid) {
                e.preventDefault();
                showToast('Please fill in all required fields', 'error');
            }
        });
    });

    // =====================================================
    // DROPDOWN AUTO-CLOSE
    // =====================================================
    document.addEventListener('click', function(e) {
        if (!e.target.closest('.dropdown, .btn-group')) {
            document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
                menu.classList.remove('show');
            });
        }
    });

    // =====================================================
    // PAGE LOAD COMPLETE
    // =====================================================
    window.addEventListener('load', function() {
        // Add fade-in animation to main content
        const contentArea = document.querySelector('.content-area');
        if (contentArea) {
            contentArea.style.opacity = '0';
            contentArea.style.transform = 'translateY(10px)';
            contentArea.style.transition = 'all 0.5s ease';
            setTimeout(() => {
                contentArea.style.opacity = '1';
                contentArea.style.transform = 'translateY(0)';
            }, 100);
        }
    });

    // =====================================================
    // CONSOLE MESSAGE
    // =====================================================
    console.log('%cTennis Shop Admin Panel', 'font-size: 24px; font-weight: bold; color: #6366f1;');
    console.log('%cModern & Professional Admin Interface', 'font-size: 14px; color: #6b7280;');
    console.log('%c✓ All systems initialized successfully', 'font-size: 12px; color: #10b981;');

})();
