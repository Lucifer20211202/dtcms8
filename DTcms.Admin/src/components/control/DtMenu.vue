<script>
	import { h,reactive,watch } from "vue"
	import { ElCollapseTransition } from 'element-plus'
	import router from "../../router"
	
	export default {
		name: 'DtMenu',
		props: {
			mini: {
				type: Boolean,
				default: false
			},
			selected: {
				type: String,
				default: null
			},
			data: {
				type: Array,
				default: () => {
					return []
				},
			}
		},
		setup(props, {emit}) {
			//属性变量
			const datas = reactive({
				isLoad: true, //首次加载
				listData: [],
			})
			//创建子节点HTML
			const elements = (rootItem, parentItem, sData, h) => {
				return h(ElCollapseTransition, {}, {
					default: () => (!props.mini && parentItem.parentId === 0) || parentItem.isExpand ? 
					h('ul',
					{
						style: {
							display: !props.mini && parentItem.parentId === 0 ? 'block' : parentItem.isExpand ? 'block' : 'none'
						}
					}, [
						//循环遍历
						sData.map((item, index) => {
							//频道页需要传参频道ID过去
							let linkUrl = item.href;
							if(item.href != null && item.href.length > 0 && item.channelId > 0) {
								linkUrl += `/${item.channelId}`;
							}
							return h('li', [
								h('a', {
									class: item.children.length == 0 && item.isSelected ? 'selected' : null,
									onClick: function (e) {
										if (item.children.length == 0) {
											localStorage.setItem('dtcms_admin_menu_url', linkUrl);//存入本地
											selectItem(datas.listData, rootItem, item);
											//跳转页面
											if(!!linkUrl) {
												router.push(linkUrl);
											}
										}
										if ((item.children.length == 0) || typeof (item.isExpand) === "undefined") {
											return false;
										}
										item.isExpand = !item.isExpand
										if (item.isExpand) {
											sData.map(function (sitem, sindex) {
												if (index != sindex) {
													sitem.isExpand = false;
												}
											})
										}
									},
								}, [
									h('i', { class: !!item.icon ? item.icon : item.children.length > 0 ? 'iconfont icon-folder' : 'iconfont icon-file' }),
									h('span', item.text),
									item.children.length > 0 ? h('b', { class: item.isExpand ? 'iconfont icon-open' : 'iconfont icon-close' }) : null,
								]),
								item.children && item.children.length > 0 ? elements(rootItem, item, item.children, h) : null
							])
						})
					]): null
				})
			}
			//选中菜单项
			const selectItem = (sData, rootItem, currItem) => {
				sData.map((item) => {
					item.isSelected = false
					if (currItem === item) {
						currItem.isSelected = true
						rootItem.isSelected = true
					} else {
						item.isSelected = false
					}
					if (item.children && item.children.length > 0) {
						selectItem(item.children, rootItem, currItem)
					}
				})
			}
			//根据存储的URL设置选中菜单项
			const selectUrl = (sData, href) => {
				sData.map((item) => {
					//频道页需要传参频道ID过去
					let linkUrl = item.href;
					if(item.href != null && item.href.length > 0 && item.channelId > 0) {
						linkUrl += `/${item.channelId}`
					}
					//检查是否与跳转相同
					if (item.children.length == 0 && linkUrl == href) {
						//console.log('找到菜单...' + item.href)
						//选中当前节点
						item.isSelected = true
						//跳转链接
						if (datas.isLoad && !!linkUrl && linkUrl != '#') {
							router.push(linkUrl)
						}
						//查找所有的父节点
						let parentNodes = findAllParent(item, datas.listData, [], 0);
						parentNodes.map((pNode) => {
							if (pNode.parentId === 0) {
								pNode.isSelected = true
								pNode.isExpand = true
							} else {
								pNode.isExpand = true
							}
						})
						//查找相邻的父节点，闭合
						if(parentNodes.length > 0) {
							let adjoinList = findAdjoin(datas.listData, parentNodes[0].parentId);
							if(adjoinList) {
								adjoinList.map((node) => {
									if(node.id != item.parentId) {
										node.isExpand = false
									}
								})
							}
						}
					} else {
						item.isSelected = false
					}
					if (item.children && item.children.length > 0) {
						selectUrl(item.children, href)
					}
				})
			}
			//迭代查找所有父节点
			const findAllParent = (node, tree, parentNodes, index) => {
				if (!node || node.parentId === 0) {
					return
				}
				findParent(node, parentNodes, tree)
				let parntNode = parentNodes[index]
				findAllParent(parntNode, tree, parentNodes, ++index)
				return parentNodes
			}
			//迭代查找上一级至根父节点
			const findParent = (node, parentNodes, tree) => {
				for (let i = 0; i < tree.length; i++) {
					let item = tree[i]
					if (item.id === node.parentId) {
						parentNodes.push(item)
						return
					}
					if (item.children && item.children.length > 0) {
						findParent(node, parentNodes, item.children)
					}
				}
			}
			//迭代查找所有相邻的节点
			const findAdjoin = (tree, parentId) => {
				for (let i = 0; i < tree.length; i++) {
					let item = tree[i]
					if (item.id === parentId) {
						return item.children
					}
					if (item.children && item.children.length > 0) {
						findAdjoin(item.children, parentId)
					}
				}
			}
			//渲染完成后初始化菜单
			const initData = () => {
				let href = localStorage.getItem('dtcms_admin_menu_url');
				//先展开一级菜单下的所有第一个子节点
				datas.listData.map((node) => {
					if(node.children && node.children[0]) {
						node.children[0].isExpand = true
					}
				});
				if (props.selected) {
					//选中指定的菜单
					selectUrl(datas.listData, props.selected)
				} else if (!!href) {
					//选中本地的菜单
					selectUrl(datas.listData, href);
				}else if (!props.mini && datas.listData[0]){
					datas.listData[0].isSelected = true
				}
			}
			
			//监视mini
			watch(() => props.mini, (newVal, oldVal) => {
				if (!newVal && datas.listData.length > 0) {
					const nodeObj = datas.listData.find(item => item.isSelected);
					if(nodeObj){
						nodeObj.isSelected = true
					}else{
						datas.listData[0].isSelected = true
					}
				}
			})
			//监视data赋值的方法，不需要深度监听
			watch(() => props.data, (newVal, oldVal) => {
				if (newVal) {
					datas.listData = props.data
					initData()
					//设置首次加载完成
					datas.isLoad = false
				}
			})
			
			//需要返回props给其它钩子使用
			return { props,datas,elements }
		},
		render() {
			let self = this;
			return h('div', { 'class': self.props.mini ? 'mini' : '' }, [
				//循环遍历
				self.datas.listData.map((item, index) => {
					//如果是第一层，需要添加DIV
					if (item.parentId === 0) {
						/*如果是简洁模式下，展开所有第一层*/
						if (self.mini) {
							item.isExpand = true
						}
						//创建VDOM对象
						return h('div', {
							'class': item.isSelected ? 'list-group selected' : 'list-group',
						}, [
							h('h1',
								{
									title: item.text,
									onClick: function() {
										if(!self.mini && !item.isSelected) {
											item.isSelected = true
											//闭合相邻节点
											self.datas.listData.map(function (sitem, sindex) {
												if (index != sindex) {
													sitem.isSelected = false;
												}
											});
										}
									}
								}, [
								h('i', 
								{
									'class': item.icon,
								})
							]),
							h('div', { 'class': 'list-wrap' },
								[
									h('h2', [
										h('i', { 'class': item.icon }),
										h('span', item.text),
										h('b', { 'class': 'iconfont icon-arrow-down' })
									]
									),
									item.children && item.children.length > 0 ? self.elements(item, item, item.children, h) : null
								]
							)
						]);
					}
				})
			]);
		}
	}
</script>

<style lang="scss">
	/*导航普通样式*/
	.sidebar-nav {
		height: 100%;
		padding-top: 8px;
		background: #fff;
		overflow: hidden;
		.list-group{
			display: block !important;
			margin: 0;
			padding: 0 4px 8px;
			width: 44px;
			height: 44px;
			&:hover {
				h1 {
					border-radius: 4px;
					background: #d4e3ff;
				}
			}
			&.selected {
				h1 {
					border-radius: 4px;
					background: #0e70d5;
					i {
						color: #f3f3f3;
					}
				}
				.list-wrap {
					display: block;
				}
			}
			.list-wrap {
				display: none;
				position: absolute;
				top: 0;
				left: 46px;
				bottom: 0;
				padding: 0 10px;
				width: 160px;
				background: #fff;
				box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
				z-index: 3;
				overflow: auto;
				> ul {
					margin-left: 0;
				}
			}
			h1 {
				display: block;
				margin: 0;
				padding: 10px;
				width: 16px;
				height: 16px;
				line-height: 14px;
				font-weight: 500;
				cursor: pointer;
				-webkit-transition-duration: 0.3s;
				transition-duration: 0.3s;
				overflow: hidden;
				i {
					display: inline-block;
					color: #0e70d5;
					font-size: 16px;
					vertical-align: top;
				}
			}
			h2 {
				position: relative;
				display: block;
				margin: 0 -10px;
				padding: 0 10px;
				color: #606266;
				font-size: 14px;
				font-weight: 500;
				line-height: 46px;
				height: 46px;
				cursor: pointer;
				box-shadow: 0px 1px 1px 0px rgba(0,0,0,.05);
				background: #fff;
				i {
					display: inline-block;
					margin-right: 5px;
					color: #0e70d5;
					font-size: 14px;
					vertical-align: baseline;
				}
				b {
					position: absolute;
					display: block;
					top: 15px;
					right: 10px;
					width: 20px;
					height: 20px;
					color: #c0c0c0;
					font-size: 10px;
					text-align: center;
					line-height: 20px;
				}
			}
			ul {
				margin-left: 17px;
				li {
					padding: 1px 0;
					a {
						position: relative;
						display: block;
						padding: 8px 20px 8px 8px;
						color: #606266;
						font-size: 14px;
						height: 20px;
						line-height: 20px;
						cursor: pointer;
						white-space: nowrap;
						text-overflow: ellipsis;
						text-decoration: none;
						-webkit-transition-duration: 0.3s;
						transition-duration: 0.3s;
						overflow: hidden;
						&.selected {
							color: #f3f3f3;
							border-radius: 4px;
							background: #0e70d5;
							>i, >b {
								color: #f3f3f3;
							}
							&:hover {
								background: #0e70d5;
							}
						}
						&:hover {
							border-radius: 4px;
							background: #eef9fe;
						}
						> i {
							display: block;
							float: left;
							margin: 3px 5px 3px 0;
							width: 14px;
							height: 14px;
							color: #0e70d5;
							font-size: 14px;
							line-height: 14px;
							text-align: center;
							overflow: hidden;
						}
						> b {
							position: absolute;
							display: block;
							top: 11px;
							right: 4px;
							width: 14px;
							height: 14px;
							color: #0e70d5;
							font-size: 12px;
							text-align: center;
							line-height: 14px;
							-webkit-transform: scale(0.833);
						}
					}
				}
			}
		}
	}
	/*导航迷你样式*/
	.sidebar-nav.mini {
		box-shadow: 0 2px 5px 0 rgba(0,0,0,.1);
		margin-right: 1px;
		.list-group {
			&.selected {
				&:hover {
					h1 {
						background: #0e70d5;
					}
				}
				.list-wrap {
					display: none;
				}
			}
			&:hover {
				h1 {
					background: #d4e3ff;
					border-radius: 4px;
				}
				.list-wrap {
					display: block;
				}
			}
		}
	}
</style>